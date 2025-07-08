using Microsoft.EntityFrameworkCore;
using WaterBillingApp.Data.Entities;

namespace WaterBillingWebAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Meter> Meters { get; set; }
        public DbSet<Consumption> Consumptions { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<TariffBracket> TariffBrackets { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<MeterRequest> MeterRequests { get; set; }
    }
}
