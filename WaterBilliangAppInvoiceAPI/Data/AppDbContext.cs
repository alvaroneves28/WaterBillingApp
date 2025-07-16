using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WaterBillingApp.Data.Entities;

namespace WaterBilliangAppInvoiceAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }
     
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Meter> Meters { get; set; }
        public DbSet<Consumption> Consumptions { get; set; }

    }
}
