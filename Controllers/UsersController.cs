using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shipping_api.Data;

namespace shipping_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;

        public UsersController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var role = User.FindFirstValue("role") ?? User.FindFirstValue(ClaimTypes.Role);
            if (role != "admin") return Forbid();

            var users = _db.Users
                .Where(u => u.Role == "user")
                .Select(u => new { u.Id, u.Username, u.Role })
                .ToList();

            return Ok(users);
        }
    }
}