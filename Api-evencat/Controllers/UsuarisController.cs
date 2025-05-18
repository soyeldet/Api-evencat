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
    public class UsuarisController : ApiController
    {
        private evencatEntities1 db = new evencatEntities1();

        // GET: api/Usuaris
        public IQueryable<Usuaris> GetUsuaris()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.Usuaris;
        }

        // GET: api/Usuaris/5
        [ResponseType(typeof(Usuaris))]
        public async Task<IHttpActionResult> GetUsuaris(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            Usuaris usuaris = await db.Usuaris.FindAsync(id);
            if (usuaris == null)
            {
                return NotFound();
            }

            return Ok(usuaris);
        }

        // GET: api/Usuaris/5
        [HttpGet]
        [Route("api/Usuaris/organizer/{id}")]
        [ResponseType(typeof(Organizer))]
        public async Task<IHttpActionResult> GetOrganitzador(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;

            Usuaris usuari = await db.Usuaris.FindAsync(id);
            if (usuari == null)
            {
                return NotFound();
            }

            var organizer = new Organizer
            {
                UserId = usuari.usuari_id,
                UserName = usuari.nom,
                ImageUrl = usuari.url_imagen,
            };

            return Ok(organizer);
        }

        // PUT: api/Usuaris/{id}/UpdateProfile
        [HttpPut]
        [Route("api/Usuaris/{id}/UpdateProfile")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> UpdateUserProfile(int id, [FromBody] UserUpdate updatedUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await db.Usuaris.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                user.nom = updatedUser.UserName;
                user.email = updatedUser.UserEmail;
                user.password_hash = updatedUser.Password;

                await db.SaveChangesAsync();

                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        // PUT: api/Usuaris/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUsuaris(int id, Usuaris usuaris)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != usuaris.usuari_id)
            {
                return BadRequest();
            }

            db.Entry(usuaris).State = System.Data.Entity.EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarisExists(id))
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

        // POST: api/User
        [HttpPost]
        [Route("api/Usuaris")]
        public async Task<IHttpActionResult> CreateUser(User user)
        {
            // Paso 1: Crear el objeto Usuario
            var userRecieved = new Usuaris

            {
                email = user.UserEmail,
                nom = user.UserName,
                password_hash = user.Password,
                rol = "UsuariNormal",
                url_imagen = user.ImageUrl ?? "",
                data_registre = DateTime.Now,
            };

            // Paso 2: Añadir el usuario a la base de datos
            db.Usuaris.Add(userRecieved);
            await db.SaveChangesAsync(); // Guarda el usuario en la tabla Usuarios

            return Ok(userRecieved);  
        }

        // POST: api/Usuaris/Login
        [HttpPost]
        [Route("api/Usuaris/Login")]
        public async Task<IHttpActionResult> Login(Classes.Classes userRecieved)
        {
            var user = await db.Usuaris
                .FirstOrDefaultAsync(u => u.email == userRecieved.UserEmail);

            if (user == null)
            {
                return Unauthorized(); // No existe un usuario con ese correo
            }

            if (user.password_hash != userRecieved.Password)
            {
                return Unauthorized(); // Contraseña incorrecta
            }

            var usuarioRecibidoCompleto = new User
            {
                UserId = user.usuari_id,
                UserName = user.nom,
                UserEmail = user.email,
                Password = user.password_hash,
                Rol = user.rol,
                ImageUrl = user.url_imagen,
                Description = user.descripcio
            };

            return Ok(usuarioRecibidoCompleto);
        }

        // PUT: api/Usuaris/5/Descripcio
        [HttpPut]
        [Route("api/Usuaris/{id}/Descripcio")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDescripcionUsuario(int id, [FromBody] string descripcio)
        {
            if (string.IsNullOrEmpty(descripcio))
            {
                descripcio = ("");
            }

            try
            {
                var usuario = await db.Usuaris.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound();
                }

                usuario.descripcio = descripcio;
                await db.SaveChangesAsync();

                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT: api/Usuaris/5/Nom
        [HttpPut]
        [Route("api/Usuaris/{id}/Nom")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutNombrenUsuario(int id, [FromBody] string nom)
        {
            if (string.IsNullOrEmpty(nom))
            {
                nom = ("");
            }

            try
            {
                var usuario = await db.Usuaris.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound();
                }

                usuario.nom = nom;
                await db.SaveChangesAsync();

                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT: api/Usuaris/{id}/ImageUrl
        [HttpPut]
        [Route("api/Usuaris/{id}/ImageUrl")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutImageUrlUsuario(int id, [FromBody] string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return BadRequest("La URL de imagen no puede estar vacía.");
            }

            try
            {
                var usuario = await db.Usuaris.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound();
                }

                usuario.url_imagen = imageUrl;
                await db.SaveChangesAsync();

                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        // DELETE: api/Usuaris/5
        [ResponseType(typeof(Usuaris))]
        public async Task<IHttpActionResult> DeleteUsuaris(int id)
        {
            Usuaris usuaris = await db.Usuaris.FindAsync(id);
            if (usuaris == null)
            {
                return NotFound();
            }

            db.Usuaris.Remove(usuaris);
            await db.SaveChangesAsync();

            return Ok(usuaris);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UsuarisExists(int id)
        {
            return db.Usuaris.Count(e => e.usuari_id == id) > 0;
        }
    }
}