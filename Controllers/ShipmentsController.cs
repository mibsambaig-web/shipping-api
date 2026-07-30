using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shipping_api.Data;
using shipping_api.Models;

namespace shipping_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ShipmentsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ShipmentsController(AppDbContext db)
        {
            _db = db;
        }

        private string GetRole() =>
            User.FindFirstValue("role") ?? User.FindFirstValue(ClaimTypes.Role) ?? "";

        private int GetUserId() =>
            int.Parse(User.FindFirstValue("userId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        [HttpGet]
        public IActionResult GetAll([FromQuery] int? userId)
        {
            var currentUserId = GetUserId();
            var role = GetRole();

            if (role != "admin")
                return Ok(_db.Shipments.Where(s => s.UserId == currentUserId).ToList());

            if (userId.HasValue)
                return Ok(_db.Shipments.Where(s => s.UserId == userId.Value).ToList());

            return Ok(_db.Shipments.ToList());
        }

        [HttpPost]
        public IActionResult Create([FromBody] Shipment shipment)
        {
            var currentUserId = GetUserId();
            var role = GetRole();

            if (role != "admin")
                shipment.UserId = currentUserId;

            _db.Shipments.Add(shipment);
            _db.SaveChanges();
            return Ok(shipment);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Shipment updated)
        {
            var role = GetRole();
            if (role != "admin") return Forbid();

            var shipment = _db.Shipments.FirstOrDefault(s => s.Id == id);
            if (shipment == null) return NotFound();

            shipment.TrackingId = updated.TrackingId;
            shipment.Client = updated.Client;
            shipment.Origin = updated.Origin;
            shipment.Destination = updated.Destination;
            shipment.Status = updated.Status;
            shipment.Date = updated.Date;

            _db.SaveChanges();
            return Ok(shipment);
        }
    }
}