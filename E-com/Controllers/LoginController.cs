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
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity.Data;
using E_com.Services.interfaces; // Make sure this is correctly imported

namespace E_com.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly MyDbContext _context;
        private readonly IEmailService _emailService;

        public LoginController(IConfiguration config, MyDbContext context, IEmailService emailService)
        {
            _config = config;
            _context = context;
            _emailService = emailService;
        }

        [HttpPost]
        [Route("SignUp")]
        public async Task<ActionResult> PostUser(User user)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                {
                    return Conflict("Email already exists.");
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                user.Created_Time = DateTime.Now;
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





        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest("Email is required.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                // Consider returning Ok here to prevent email enumeration attacks
                return Ok(new { message = "If a user with this email exists, a password reset link has been sent." });
            }

            var token = GenerateJwtToken(user.Email!);

            // Generate the reset link
            // Replace this with your actual client-side URL
            var clientUrl = _config["ClientUrl:Url"] ?? throw new InvalidOperationException("ClientUrl is not configured.");
            var resetLink = $"{clientUrl}/ResetPassword?token={Uri.EscapeDataString(token)}";

            // Example email body with the reset link
            var subject = "Password Reset Request";
            var body = $@"
                    <html>
                        <body>
                            <p>To reset your password, please click the link below:</p>
                            <p><a href='{resetLink}'>Reset Password</a></p>
                            <p>If you didn't request a password reset, please ignore this email.</p>
                        </body>
                    </html>";

            // Send the email
            await _emailService.SendEmailAsync(user.Email!, subject, body);

            return Ok(new { message = "If a user with this email exists, a password reset link has been sent." });
        }



        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.NewPassword))
            {
                return BadRequest("Token and new password are required.");
            }

            var email = ValidateToken(request.Token);
            if (email == null)
            {
                return BadRequest("Invalid or expired token.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.Modified_Time = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Password has been reset successfully." });
        }

        private string? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured properly.");
            var key = Encoding.ASCII.GetBytes(jwtKey);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false, // Allowing expired tokens for password reset
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return principal.FindFirst(ClaimTypes.Email)?.Value;
            }
            catch
            {
                return null;
            }
        }

    }

    
}
