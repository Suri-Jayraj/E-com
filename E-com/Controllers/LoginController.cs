using E_com.DAL;
using E_com.Models;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration; // Make sure this is correctly imported

namespace E_com.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly MyDbContext _context;

        public LoginController(IConfiguration config, MyDbContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpPost]
        [Route("SignUp")]
        public async Task<ActionResult> PostUser(User user)
        {
            try
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                user.Created_Time = DateTime.Now;
                user.Created_By = 1; // Consider dynamically setting this based on the logged-in user
                user.Modified_By = 1; // Consider dynamically setting this based on the logged-in user
                user.Modified_Time = DateTime.Now;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "User created successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // Consider using a logger
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(Login login)
        {
            if (login == null || string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.Password))
            {
                return BadRequest("Email and password are required.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == login.Email);
            if (user != null && BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
            {
                var token = GenerateJwtToken(user.Email!); // Assuming email is not null. Consider handling potential null values.
                return Ok(new { message = "Login successful.", token });
            }
            else
            {
                return Unauthorized("Login failed.");
            }
        }

        private string GenerateJwtToken(string email)
        {
            var jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured properly.");
            var key = Encoding.ASCII.GetBytes(jwtKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, email)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


    }
}
