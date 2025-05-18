using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Api_evencat.Models;

namespace Api_evencat.Classes
{
    public class Classes
    {
        public string UserEmail { get; set; }
        public string Password { get; set; }
    }

    public class User
    {
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string Password { get; set; }
        public string Rol { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }

    public class EventDTO
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public string ImageUrl { get; set; }
        public string State { get; set; }
        public int EspaiId { get; set; }
        public int OrganitzadorId { get; set; }
    }

    public class Organizer
    {
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string ImageUrl { get; set; }

    }

    public class ReservationRequest
    {
        [Required]
        public int EventId { get; set; }

        public int? ButacaId { get; set; } // Nullable para reservas sin butaca

        [Required]
        public int UserId { get; set; }
    }

    public class ReservaResponse
    {
        public int ReservaId { get; set; }
        public int EventId { get; set; }
        public int? ButacaId { get; set; }
        public DateTime DataReserva { get; set; }
    }

    public class ReservesUsuari
    {
        public int reserva_id { get; set; }
        public int usuari_id { get; set; }
        public int event_id { get; set; }

        public virtual Usuaris Usuari { get; set; }
        public virtual Esdeveniments Esdeveniment { get; set; }
    }

    public class UserUpdate
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string Password { get; set; }
    }

    public class FriendDto
    {
        public int usuari_id { get; set; }
        public string nom { get; set; }
    }

}
