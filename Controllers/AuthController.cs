using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using shipping_api.Data;
using shipping_api.Models;

namespace shipping_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AuthController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _db.Users.FirstOrDefault(u => u.Username == request.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid username or password" });

            var token = GenerateToken(user.Username, user.Role, user.Id);
            return Ok(new { token, role = user.Role });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] LoginRequest request)
        {
            if (_db.Users.Any(u => u.Username == request.Username))
                return BadRequest(new { message = "Username already exists" });

            var user = new AppUser
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "admin"
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            return Ok(new { message = "Admin created successfully" });
        }

        [HttpPost("create-client")]
        [Authorize]
        public IActionResult CreateClient([FromBody] LoginRequest request)
        {
            var callerRole = User.FindFirstValue("role") ?? User.FindFirstValue(ClaimTypes.Role);
            if (callerRole != "admin")
                return Forbid();

            if (_db.Users.Any(u => u.Username == request.Username))
                return BadRequest(new { message = "Username already exists" });

            var user = new AppUser
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "user"
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            return Ok(new { message = "Client created successfully", userId = user.Id });
        }

        private string GenerateToken(string username, string role, int userId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SwiftCargoSecretKey123456789012"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim("role", role),
                    new Claim("userId", userId.ToString())
                },
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}