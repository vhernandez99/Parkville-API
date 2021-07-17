using CinemaAPI.Data;
using CinemaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private CinemaDbContext _dbContext;
        public MoviesController(CinemaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize]
        [HttpGet]
        public IActionResult AllMovies(string sort, int? pageNumber, int? pageSize)
        {
            var currentPageNumber= pageNumber ?? 1;
            var currentPageSize=pageSize ?? 5;
            var movies =from movie in _dbContext.Movies.Where(x=>x.Status==1)
                        select new
                        {
                            Id = movie.Id,
                            Name = movie.Name,
                            Duration = movie.Duration,
                            Language = movie.Language,
                            Rating = movie.Rating,
                            Genre = movie.Genre,
                            ImageUrl = movie.ImageUrl
                        };

            switch (sort)
            {
                case "desc":
                    return Ok(movies.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize).OrderByDescending(m=> m.Rating));
                case "asc":
                    return Ok(movies.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize).OrderBy(m => m.Rating));
                default:
                    return Ok(movies.Skip((currentPageNumber-1)* currentPageSize).Take(currentPageSize));
            } 
        }
         
        

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult MovieDetail(int id)
        {
            var movie=_dbContext.Movies.Find(id);
            if (movie == null)
            {
                return NotFound();
            }
            return Ok(movie);
        }
        [Authorize]
        [HttpGet]
        public IActionResult FindMovies(string movieName)
        {
            //Search reservationId
            //var string2 = Convert.ToInt32(movieName);
            var movies = from movie in _dbContext.Movies
                         where movie.Name.StartsWith(movieName)
                         select new
                         {
                            Id = movie.Id,
                            Name = movie.Name,
                            ImageUrl = movie.ImageUrl
                         };
            return Ok(movies);

        }


        

        //[Authorize(Roles = "Admin")]
        //[HttpPut("{id}")]
        //public IActionResult DeleteMovie(int id)
        //{
        //    var movie = _dbContext.Movies.Find(id);
        //    if (movie == null)
        //    {
        //        return NotFound("No record found against this id");
        //    }
        //    else
        //    {
        //        _dbContext.Remove(movie);
        //        _dbContext.SaveChanges();
        //        return Ok("Record deleted");
        //    }
        //}

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult UpdateImageBanner(int id, [FromForm] BannerImage movieObj)
        {
            var BannerImage = _dbContext.BannerImages.Find(id);
            if (BannerImage == null)
            {
                return NotFound("No record found against this id");
            }
            else
            {
                var guid = Guid.NewGuid();
                var filePath = Path.Combine("wwwroot", guid + ".jpg");
                if (movieObj != null)
                {
                    var fileStream = new FileStream(filePath, FileMode.Create);
                    movieObj.Image.CopyTo(fileStream);
                    BannerImage.ImageUrl = filePath.Remove(0, 7);
                }
                _dbContext.SaveChanges();
                return Ok("Record updated succesfully");
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult DeleteMovie(int id)
        {
            var movie = _dbContext.Movies.Find(id);
            if (movie == null)
            {
                return NotFound("No record found against this id");
            }
            else
            {
                movie.Status = 0;
                _dbContext.SaveChanges();
                return Ok("Record deleted");
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult AllBannerImages()
        {
            var BannerImages = from bannerImage in _dbContext.BannerImages
                               select new
                               {
                                   Id = bannerImage.BannerId,
                                   Name = bannerImage.Name,
                                   ImageUrl = bannerImage.ImageUrl
                               };
            return Ok(BannerImages);
        }

        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddMovie([FromForm] Movie movieObj)
        {
            var guid = Guid.NewGuid();
            var filePath = Path.Combine("wwwroot", guid + ".jpg");
            if (movieObj != null)
            {
                var fileStream = new FileStream(filePath, FileMode.Create);
                movieObj.Image.CopyTo(fileStream);
            }
            movieObj.ImageUrl = filePath.Remove(0, 7);
            movieObj.PlayingDate.ToShortDateString();
            movieObj.PlayingDate2.ToShortDateString();
            movieObj.PlayingDate3.ToShortDateString();
            movieObj.PlayingDate4.ToShortDateString();
            movieObj.PlayingDate5.ToShortDateString();
            movieObj.Status = 1;
            movieObj.PlayingTime.ToShortTimeString();
            movieObj.PlayingTime2.ToShortTimeString();
            movieObj.PlayingTime3.ToShortTimeString();
            movieObj.PlayingTime4.ToShortTimeString();
            movieObj.PlayingTime5.ToShortTimeString();
            _dbContext.Movies.Add(movieObj);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromForm] Movie movieObj)
        {
            var movie = _dbContext.Movies.Find(id);
            if (movie == null)
            {
                return NotFound("No record found against this id");
            }
            else
            {
                var guid = Guid.NewGuid();
                var filePath = Path.Combine("wwwroot", guid + ".jpg");
                if (movieObj != null)
                {
                    var fileStream = new FileStream(filePath, FileMode.Create);
                    movieObj.Image.CopyTo(fileStream);
                    movie.ImageUrl = filePath.Remove(0, 7);
                }
                movie.Name = movieObj.Name;
                movie.Description = movieObj.Description;
                movie.Language = movieObj.Language;
                movie.Duration = movieObj.Duration;
                movie.PlayingDate = movieObj.PlayingDate;
                movie.PlayingTime = movieObj.PlayingTime;
                movie.Rating = movieObj.Rating;
                movie.Genre = movieObj.Genre;
                movie.TrailorUrl = movieObj.TrailorUrl;
                movie.TicketPrice = movieObj.TicketPrice;
                _dbContext.SaveChanges();
                return Ok("Record updated succesfully");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddImageBanner([FromForm] BannerImage movieObj)
        {
            var guid = Guid.NewGuid();
            var filePath = Path.Combine("wwwroot", guid + ".jpg");
            if (movieObj != null)
            {
                var fileStream = new FileStream(filePath, FileMode.Create);
                movieObj.Image.CopyTo(fileStream);
            }
            movieObj.ImageUrl = filePath.Remove(0, 7);
            _dbContext.BannerImages.Add(movieObj);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }
    }
}
