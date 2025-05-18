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
    public class ButaquesController : ApiController
    {
        private evencatEntities1 db = new evencatEntities1();

        // GET: api/Butaques
        public IQueryable<Butaques> GetButaques()
        {
            db.Configuration.LazyLoadingEnabled = false;

            return db.Butaques;
        }

        // GET: api/Butaques/5
        [ResponseType(typeof(Butaques))]
        public async Task<IHttpActionResult> GetButaques(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            Butaques butaques = await db.Butaques.FindAsync(id);
            if (butaques == null)
            {
                return NotFound();
            }

            return Ok(butaques);
        }

        // PUT: api/Butaques/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutButaques(int id, Butaques butaques)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != butaques.butaca_id)
            {
                return BadRequest();
            }

            db.Entry(butaques).State = System.Data.Entity.EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ButaquesExists(id))
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

        // POST: api/Butaques
        [ResponseType(typeof(Butaques))]
        public async Task<IHttpActionResult> PostButaques(Butaques butaques)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Butaques.Add(butaques);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = butaques.butaca_id }, butaques);
        }

        // DELETE: api/Butaques/5
        [ResponseType(typeof(Butaques))]
        public async Task<IHttpActionResult> DeleteButaques(int id)
        {
            Butaques butaques = await db.Butaques.FindAsync(id);
            if (butaques == null)
            {
                return NotFound();
            }

            db.Butaques.Remove(butaques);
            await db.SaveChangesAsync();

            return Ok(butaques);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ButaquesExists(int id)
        {
            return db.Butaques.Count(e => e.butaca_id == id) > 0;
        }
    }
}