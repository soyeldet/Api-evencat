using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Api_evencat.Classes;
using Api_evencat.Models;

namespace Api_evencat.Controllers
{
    public class ReservesController : ApiController
    {
        private readonly evencatEntities1 db = new evencatEntities1();

        // GET: api/Reserves
        public IQueryable<Reserves> GetReserves()
        {
            return db.Reserves;
        }

        // GET: api/Reserves/5
        [ResponseType(typeof(Reserves))]
        public async Task<IHttpActionResult> GetReserves(int id)
        {
            Reserves reserve = await db.Reserves.FindAsync(id);
            if (reserve == null)
            {
                return NotFound();
            }
            return Ok(reserve);
        }

        // GET: api/Reserves/ByEvent/5
        [HttpGet]
        [ResponseType(typeof(List<Reserves>))]
        public async Task<IHttpActionResult> ByEvent(int eventId)
        {
            var reserves = await db.Reserves
                .Where(r => r.event_id == eventId)
                .ToListAsync();

            return Ok(reserves);
        }

        // GET: api/Reserves/AvailableSeats/5
        [HttpGet]
        [Route("api/Reserves/AvailableSeats/{eventId}")]
        [ResponseType(typeof(List<int>))]
        public async Task<IHttpActionResult> AvailableSeats(int eventId)
        {
            var evento = await db.Esdeveniments
                .Include(e => e.Espais)
                .FirstOrDefaultAsync(e => e.event_id == eventId);

            if (evento == null) return NotFound();

            var espacio = evento.Espais;

            // Si no hay cadires_fixes, devolver lista con [0]
            if (!espacio.cadires_fixes.HasValue)
            {
                return Ok(new List<int> { 0 });
            }

            var espacioId = espacio.espai_id;

            var reservedSeats = await db.Reserves
                .Where(r => r.event_id == eventId)
                .Select(r => r.butaca_id)
                .ToListAsync();

            var availableSeatIds = await db.Butaques
                .Where(b => b.espai_id == espacioId && !reservedSeats.Contains(b.butaca_id))
                .Select(b => b.butaca_id)
                .ToListAsync();

            return Ok(availableSeatIds);
        }

        // GET: api/reservations/check
        [HttpGet]
        [Route("api/reservations/check")]
        public async Task<IHttpActionResult> CheckReservation(int userId, int eventId)
        {
            try
            {
                // Verificar si existe alguna reserva para el usuario y evento
                bool hasReservation = await db.Reserves
                    .AnyAsync(r => r.usuari_id == userId && r.event_id == eventId);

                return Ok(hasReservation);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpPost]
        [Route("api/Reserves/CreateWithDetails")]
        [ResponseType(typeof(ReservaResponse))]
        public async Task<IHttpActionResult> CreateWithDetails(ReservationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1. Verificar existencia del evento
            var evento = await db.Esdeveniments
                .Include(e => e.Espais)
                .FirstOrDefaultAsync(e => e.event_id == request.EventId);

            if (evento == null)
                return NotFound();

            // 2. Verificar usuario
            var usuario = await db.Usuaris.FindAsync(request.UserId);
            if (usuario == null)
                return NotFound();

            bool tieneReserva = await db.Reserves
            .AnyAsync(r => r.event_id == request.EventId && r.usuari_id == request.UserId);

            if (tieneReserva)
                return BadRequest("El usuario ya tiene una reserva para este evento");

            // 3. Lógica de reserva
            Reserves nuevaReserva;

            if (request.ButacaId.HasValue && request.ButacaId != 0)
            {
                // Reserva con butaca específica: validar que no esté ocupada
                bool butacaOcupada = await db.Reserves.AnyAsync(r =>
                    r.event_id == request.EventId &&
                    r.butaca_id == request.ButacaId);

                if (butacaOcupada)
                    return Conflict();

                nuevaReserva = new Reserves
                {
                    event_id = request.EventId,
                    butaca_id = request.ButacaId.Value,
                    usuari_id = request.UserId,
                    data_reserva = DateTime.UtcNow
                };
            }
            else
            {
                // Reserva sin butaca (aleatoria o sin asiento fijo)
                // Solo limitar si cadires_fixes está definido y no es un caso especial (id = 0)
                if (request.ButacaId != 0 && evento.Espais.cadires_fixes.HasValue)
                {
                    var reservasExistentes = await db.Reserves
                        .CountAsync(r => r.event_id == request.EventId && !r.butaca_id.HasValue);

                    if (reservasExistentes >= evento.Espais.cadires_fixes.Value)
                        return Conflict();
                }

                nuevaReserva = new Reserves
                {
                    event_id = request.EventId,
                    butaca_id = null, // importante: null, porque 0 no es una butaca válida
                    usuari_id = request.UserId,
                    data_reserva = DateTime.UtcNow
                };
            }

            // 4. Guardar en base de datos
            db.Reserves.Add(nuevaReserva);
            await db.SaveChangesAsync();

            // 5. Responder
            var response = new ReservaResponse
            {
                ReservaId = nuevaReserva.reserva_id,
                EventId = nuevaReserva.event_id,
                ButacaId = nuevaReserva.butaca_id,
                DataReserva = nuevaReserva.data_reserva.Value
            };

            return Ok(response);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}