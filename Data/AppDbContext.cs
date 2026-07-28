using Microsoft.EntityFrameworkCore;
using shipping_api.Models;

namespace shipping_api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<AppUser> Users { get; set; }
    }
}