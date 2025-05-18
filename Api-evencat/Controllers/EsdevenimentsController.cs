using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Api_evencat.Classes;
using Api_evencat.Models;

namespace Api_evencat.Controllers
{
    public class EsdevenimentsController : ApiController
    {
        private evencatEntities1 db = new evencatEntities1();

        // GET: api/Esdeveniments
        public async Task<IHttpActionResult> GetEsdeveniments()
        {
            db.Configuration.LazyLoadingEnabled = false;
            var events = await db.Esdeveniments
                     .Where(e => e.data_hora >= DateTime.Now)
                     .ToListAsync();

            if (events == null || events.Count == 0)
            {
                return NotFound();
            }

            // Mapear los eventos a DTO
            var eventDTOs = events.Select(e => new EventDTO
            {
                EventId = e.event_id,
                Name = e.nom,
                Description = e.descripcio,
                Date = e.data_hora.ToString("yyyy-MM-dd HH:mm:ss"),
                ImageUrl = e.imatge_promocional_url,
                State = e.estat,
                EspaiId = e.espai_id,
                OrganitzadorId = e.organitzador_id
            }).ToList();

            return Ok(eventDTOs);
        }

        // GET: api/Esdeveniments/5
        [ResponseType(typeof(Esdeveniments))]
        public async Task<IHttpActionResult> GetEsdeveniments(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            Esdeveniments esdeveniments = await db.Esdeveniments.FindAsync(id);
            if (esdeveniments == null)
            {
                return NotFound();
            }

            if (esdeveniments.data_hora < DateTime.UtcNow)
            {
                return NotFound();
            }

            return Ok(esdeveniments);
        }

        // PUT: api/Esdeveniments/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutEsdeveniments(int id, Esdeveniments esdeveniments)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != esdeveniments.event_id)
            {
                return BadRequest();
            }

            db.Entry(esdeveniments).State = System.Data.Entity.EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EsdevenimentsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Esdeveniments
        [ResponseType(typeof(Esdeveniments))]
        public async Task<IHttpActionResult> PostEsdeveniments(Esdeveniments esdeveniments)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validación simple
            if (string.IsNullOrWhiteSpace(esdeveniments.nom) ||
                string.IsNullOrWhiteSpace(esdeveniments.descripcio) ||
                esdeveniments.data_hora == default(DateTime) ||
                esdeveniments.espai_id <= 0 ||
                esdeveniments.organitzador_id <= 0)
            {
                return BadRequest("Faltan datos obligatorios.");
            }

            // Puedes asignar valores por defecto si vienen vacíos
            if (string.IsNullOrWhiteSpace(esdeveniments.text_promocional))
                esdeveniments.text_promocional = "";

            if (string.IsNullOrWhiteSpace(esdeveniments.imatge_promocional_url))
                esdeveniments.imatge_promocional_url = "https://media.istockphoto.com/id/1295114854/es/foto/sillones-rojos-vac%C3%ADos-de-un-teatro-listo-para-un-espect%C3%A1culo.jpg?s=612x612&w=0&k=20&c=zwRpOfKHmNHrwgveJZKl3hVVCR-Sucde8FC4V-sT2cw=";

            if (string.IsNullOrWhiteSpace(esdeveniments.estat))
                esdeveniments.estat = "actiu"; // ejemplo de estado por defecto

            db.Esdeveniments.Add(esdeveniments);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = esdeveniments.event_id }, esdeveniments);
        }

        // GET: api/Esdeveniments/Reservats/5
        [HttpGet]
        [Route("api/Esdeveniments/Reservats/{usuariId}")]
        public async Task<IHttpActionResult> GetEsdevenimentsReservatsPerUsuari(int usuariId)
        {
            db.Configuration.LazyLoadingEnabled = false;

            var reserves = await db.Reserves
                .Where(r => r.usuari_id == usuariId)
                .Select(r => r.event_id)
                .ToListAsync();

            if (reserves == null || !reserves.Any())
            {
                return Ok(new List<EventDTO>()); // Devuelve lista vacía si no hay reservas
            }

            var esdeveniments = await db.Esdeveniments
                .Where(e => reserves.Contains(e.event_id))
                .ToListAsync();

            var eventDTOs = esdeveniments.Select(e => new EventDTO
            {
                EventId = e.event_id,
                Name = e.nom,
                Description = e.descripcio,
                Date = e.data_hora.ToString("yyyy-MM-dd HH:mm:ss"),
                ImageUrl = e.imatge_promocional_url,
                State = e.estat,
                EspaiId = e.espai_id,
                OrganitzadorId = e.organitzador_id
            }).ToList();

            return Ok(eventDTOs);
        }



        // DELETE: api/Esdeveniments/5
        [ResponseType(typeof(Esdeveniments))]
        public async Task<IHttpActionResult> DeleteEsdeveniments(int id)
        {
            Esdeveniments esdeveniments = await db.Esdeveniments.FindAsync(id);
            if (esdeveniments == null)
            {
                return NotFound();
            }

            db.Esdeveniments.Remove(esdeveniments);
            await db.SaveChangesAsync();

            return Ok(esdeveniments);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EsdevenimentsExists(int id)
        {
            return db.Esdeveniments.Count(e => e.event_id == id) > 0;
        }
    }
}