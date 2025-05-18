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
using Api_evencat.Models;

namespace Api_evencat.Controllers
{
    public class EspaisController : ApiController
    {
        private evencatEntities1 db = new evencatEntities1();

        // GET: api/Espais
        public IQueryable<object> GetEspais()
        {
            db.Configuration.LazyLoadingEnabled = false;

            return db.Espais.Select(e => new {
                e.espai_id,
                e.nom,
                e.ubicacio,
                e.cadires_fixes
            });
        }

        // GET: api/Espais/5
        [ResponseType(typeof(Espais))]
        public async Task<IHttpActionResult> GetEspais(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            // Incluir los asientos (Butaques) del espacio
            Espais espais = await db.Espais
                .Include(e => e.Butaques)
                .FirstOrDefaultAsync(e => e.espai_id == id);

            if (espais == null)
            {
                return NotFound();
            }

            return Ok(espais);
        }

        // PUT: api/Espais/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutEspais(int id, Espais espais)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != espais.espai_id)
            {
                return BadRequest();
            }

            db.Entry(espais).State = System.Data.Entity.EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EspaisExists(id))
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

        // POST: api/Espais
        [ResponseType(typeof(Espais))]
        public async Task<IHttpActionResult> PostEspais(Espais espais)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Espais.Add(espais);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = espais.espai_id }, espais);
        }

        // DELETE: api/Espais/5
        [ResponseType(typeof(Espais))]
        public async Task<IHttpActionResult> DeleteEspais(int id)
        {
            Espais espais = await db.Espais.FindAsync(id);
            if (espais == null)
            {
                return NotFound();
            }

            db.Espais.Remove(espais);
            await db.SaveChangesAsync();

            return Ok(espais);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EspaisExists(int id)
        {
            return db.Espais.Count(e => e.espai_id == id) > 0;
        }
    }
}