using AuthenticationPlugin;
using CinemaAPI.Data;
using CinemaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CinemaAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    
    public class UsersController : ControllerBase
    {
        private CinemaDbContext _dbcontext;
        private IConfiguration _configuration;
        private readonly AuthService _auth;
        public UsersController(CinemaDbContext dbcontext, IConfiguration configuration)
        {
            _configuration = configuration;
            _auth = new AuthService(_configuration);
            _dbcontext = dbcontext;
        }
        [HttpPost]
        public IActionResult Register([FromBody] User user)
        {
            var userWithSameEmail = _dbcontext.Users.Where(u => u.Email == user.Email).SingleOrDefault();
            if (userWithSameEmail != null)
            {
                return BadRequest("User with same email already exists");
            }
            var userObj = new User
            {
                Name = user.Name,
                Email = user.Email,
                Password = SecurePasswordHasherHelper.Hash( user.Password),
                Role = "Users",
                PhoneNumber = user.PhoneNumber,
                TokenFirebase = user.TokenFirebase
            };
            _dbcontext.Users.Add(userObj);
            _dbcontext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }
        
        [HttpPost]
        public IActionResult Login([FromBody]User user)
        {
            var userEmail= _dbcontext.Users.FirstOrDefault(u => u.Email == user.Email);
            if (string.IsNullOrEmpty(user.TokenFirebase))
            {

            }
            if (userEmail == null)
            {
                return NotFound();
            }
            if (!SecurePasswordHasherHelper.Verify(user.Password, userEmail.Password))
            {
                return Unauthorized();
            }
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role,userEmail.Role)
            };
            var token = _auth.GenerateAccessToken(claims);
            return new ObjectResult(new
            {
                access_token = token.AccessToken,
                expires_in = token.ExpiresIn,
                token_type = token.TokenType,
                creation_Time = token.ValidFrom,
                expiration_Time = token.ValidTo,
                user_id=userEmail.Id,
                User_Name = userEmail.Name
            });
        }

        [HttpGet]
        public IActionResult GetAllTokens()
        {
            var tokens = from users in _dbcontext.Users
                         select new
                         {
                             token = users.TokenFirebase
                         };
            return Ok(tokens);

        }

    }
}
