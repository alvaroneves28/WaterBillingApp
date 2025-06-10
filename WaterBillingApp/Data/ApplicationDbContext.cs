using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using REMOVED.Data.Entities;

namespace REMOVED.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Meter> Meters { get; set; }
        public DbSet<Consumption> Consumptions { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<TariffBracket> TariffBrackets { get; set; }
        public DbSet<MeterRequest> MeterRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //definir MaxVolume como opcional (pode ser null para último escalão)
            modelBuilder.Entity<TariffBracket>()
                .Property(t => t.MaxVolume)
                .IsRequired(false);

            //relação 1:1 entre ApplicationUser e Customer
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Customer)
                .WithOne(c => c.ApplicationUser)
                .HasForeignKey<Customer>(c => c.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Invoice>()
                .Property(i => i.TotalAmount)
                .HasPrecision(10, 2);

            modelBuilder.Entity<TariffBracket>()
                .Property(t => t.PricePerCubicMeter)
                .HasPrecision(10, 4);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Meters)
                .WithOne(m => m.Customer)
                .HasForeignKey(m => m.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
