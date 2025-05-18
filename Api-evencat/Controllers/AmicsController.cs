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
    public class AmicsController : ApiController
    {
        private evencatEntities1 db = new evencatEntities1();

        // GET: api/Amics
        public IQueryable<Amics> GetAmics()
        {
            return db.Amics;
        }

        // GET: api/friends/5
        [HttpGet]
        [Route("api/friends/{userId}")]
        public async Task<IHttpActionResult> GetFriends(int userId)
        {
            try
            {
                var friends = await db.Amics
                    .Include(a => a.Usuaris)
                    .Include(a => a.Usuaris1)
                    .Where(a => a.usuari1_id == userId || a.usuari2_id == userId)
                    .Select(a => a.usuari1_id == userId
                        ? new FriendDto { usuari_id = a.usuari2_id, nom = a.Usuaris1.nom }
                        : new FriendDto { usuari_id = a.usuari1_id, nom = a.Usuaris.nom })
                    .Distinct()
                    .ToListAsync();


                return Ok(friends);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }

        // POST: api/Amics
        [ResponseType(typeof(Amics))]
        public async Task<IHttpActionResult> PostAmics(Amics amics)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Amics.Add(amics);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = amics.id }, amics);
        }


        // DELETE: api/Amics/5
        [ResponseType(typeof(Amics))]
        public async Task<IHttpActionResult> DeleteAmics(int id)
        {
            Amics amics = await db.Amics.FindAsync(id);
            if (amics == null)
            {
                return NotFound();
            }

            db.Amics.Remove(amics);
            await db.SaveChangesAsync();

            return Ok(amics);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AmicsExists(int id)
        {
            return db.Amics.Count(e => e.id == id) > 0;
        }
    }
}