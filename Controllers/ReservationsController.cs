using CinemaAPI.Data;
using CinemaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private CinemaDbContext _dbContext;
        public ReservationsController(CinemaDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [Authorize]
        [HttpPost]
        public IActionResult CreateReservation(Reservation reservationObj )
        {          
            reservationObj.ReservationTime = DateTime.Now;
            _dbContext.Reservations.Add(reservationObj);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult UpdateIfTicketIsUsed(int id, [FromBody] Reservation reservationObj)
        {
            var reservation = _dbContext.Reservations.Find(id);
            if (reservation == null)
            {
                return NotFound("Not reservation found against this id");
            }
            else
            {
                reservation.IsUsed = reservationObj.IsUsed;
                _dbContext.SaveChanges();
                return Ok("Record updated successfully");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult UpdateIfTicketIsPaid(int id, [FromBody] Reservation reservationObj)
        {
            var reservation = _dbContext.Reservations.Find(id);
            if (reservation == null)
            {
                return NotFound("Not reservation found against this id");
            }
            else
            {
                reservation.IsPaid = reservationObj.IsPaid;
                _dbContext.SaveChanges();
                return Ok("Record updated successfully");
            }
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        public IActionResult GetReservations()
        {
            var reservations=from reservation in _dbContext.Reservations
            join customer in _dbContext.Users on reservation.UserId equals customer.Id
            join movie in _dbContext.Movies on reservation.MovieId equals movie.Id
            select new
            {
                Id= reservation.Id,
                ReservationTime= reservation.ReservationTime,
                CustomerName = customer.Name,
                MovieName=movie.Name
            };
            return Ok(reservations);
        }
        [Authorize(Roles ="Admin")]
        [HttpGet("{id}")]
        public IActionResult GetReservationDetail(int id)
        {
            var reservationResult = (from reservation in _dbContext.Reservations
                               join customer in _dbContext.Users on reservation.UserId equals customer.Id
                               join movie in _dbContext.Movies on reservation.MovieId equals movie.Id
                               where reservation.Id == id
                               select new
                               {
                                   Id = reservation.Id,
                                   ReservationTime = reservation.ReservationTime,
                                   CustomerName = customer.Name,
                                   MovieName = movie.Name,
                                   Email = customer.Email,
                                   Qty = reservation.Qty,
                                   reservation.Price,                     
                                   PlayingDate=movie.PlayingDate,
                                   PlayingTime = movie.PlayingTime,
                                   Phone=customer.PhoneNumber,
                                   IsPaid=reservation.IsPaid,
                                   IsUsed = reservation.IsUsed
                               }).FirstOrDefault();
            
            return Ok(reservationResult);
        }
        [HttpGet("{id}")]
        public IActionResult GetMyReservations(int id)
        {
            var myReservations = (from reservation in _dbContext.Reservations
                                  join customer in _dbContext.Users on reservation.UserId equals customer.Id
                                  join movie in _dbContext.Movies on reservation.MovieId equals movie.Id
                                  where reservation.UserId == id
                                  select new
                                  {
                                      ReservationId = reservation.Id,
                                      CustomerId = customer.Id,
                                      MovieName = movie.Name,
                                      CustomerName =customer.Name,
                                      ReservationTime = reservation.ReservationTime,
                                      Price = reservation.Price,
                                      PlayingDate = movie.PlayingDate,
                                      PlayingTime = movie.PlayingTime,
                                      IsPaid = reservation.IsPaid
                                  }).OrderBy(c => c.PlayingDate.Date).ThenBy(c => c.PlayingTime.TimeOfDay);
            return Ok(myReservations);
        }
        [HttpGet("{id}")]
        public IActionResult GetMyReservationDetails(int id)
        {
            var userReservation = (from reservation in _dbContext.Reservations
                                   join customer in _dbContext.Users on reservation.UserId equals customer.Id
                                   join movie in _dbContext.Movies on reservation.MovieId equals movie.Id
                                   where reservation.Id == id
                                   select new
                                   {
                                       Price = movie.TicketPrice,
                                       IsPaid = reservation.IsPaid,
                                       MovieName = movie.Name,
                                       CustomerId=customer.Id,
                                       PlayingDate = movie.PlayingDate,
                                       PlayingTime = movie.PlayingTime,
                                       ReservationId=reservation.Id
                                   }).FirstOrDefault();
            return Ok(userReservation);
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteReservations(int id)
        {
            var reservation = _dbContext.Reservations.Find(id);
            if (reservation == null)
            {
                return NotFound("No record found against this id");
            }
            else
            {
                _dbContext.Remove(reservation);
                _dbContext.SaveChanges();
                return Ok("Record deleted");
            }
        }
    }
}
