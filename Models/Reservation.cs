using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaAPI.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int Qty { get; set; }
        public double Price { get; set; }
        public DateTime ReservationTime { get; set; }
        public string HorarioDePelicula { get; set; }
        public int MovieId { get; set; }
        public int UserId { get; set; }
        [MaxLength(5)]
        public string IsPaid { get; set; }
        [MaxLength(5)]
        public string IsUsed { get; set; }
    }
}
