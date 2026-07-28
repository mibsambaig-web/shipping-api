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

        [HttpGet]
        public IActionResult GetAll()
        {
            var shipments = _db.Shipments.ToList();
            return Ok(shipments);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Shipment shipment)
        {
            _db.Shipments.Add(shipment);
            _db.SaveChanges();
            return Ok(shipment);
        }
    }
}