using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaAPI.Models
{
    public class Movie
    {
       
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public string Language { get; set; }
        public DateTime PlayingDate { get; set; }
        public DateTime PlayingDate2 { get; set; }
        public DateTime PlayingDate3 { get; set; }
        public DateTime PlayingDate4 { get; set; }
        public DateTime PlayingDate5 { get; set; }
        public DateTime PlayingTime { get; set; }
        public DateTime PlayingTime2 { get; set; }
        public DateTime PlayingTime3 { get; set; }
        public DateTime PlayingTime4 { get; set; }
        public DateTime PlayingTime5 { get; set; }
        public double TicketPrice { get; set; }
        public double Rating { get; set; }
        public string Genre { get; set; }
        public string TrailorUrl { get; set; }
        public string ImageUrl { get; set; }

        [NotMapped]
        public IFormFile Image { get; set; }
        public ICollection<Reservation> Reservations { get; set; }


    }
}
