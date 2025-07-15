using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WaterBillingApp.Data.Entities;

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
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<MeterRequest> MeterRequests { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TariffBracket>()
            .Property(t => t.MaxVolume)
            .HasPrecision(10, 3) 
            .IsRequired(false);

        modelBuilder.Entity<TariffBracket>()
            .Property(t => t.MinVolume)
            .HasPrecision(10, 3);  

        modelBuilder.Entity<TariffBracket>()
            .Property(t => t.PricePerCubicMeter)
            .HasPrecision(10, 4);

        modelBuilder.Entity<ApplicationUser>()
            .HasOne(u => u.Customer)
            .WithOne(c => c.ApplicationUser)
            .HasForeignKey<Customer>(c => c.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Meters)
            .WithOne(m => m.Customer)
            .HasForeignKey(m => m.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.Property(e => e.TotalAmount)
                .HasPrecision(18, 2);
        });

        modelBuilder.Entity<Consumption>(entity =>
        {
            entity.Property(c => c.Volume)
                .HasPrecision(18, 2);
        });

        modelBuilder.Entity<Consumption>()
                    .HasOne(c => c.TariffBracket)
                    .WithMany()
                    .HasForeignKey(c => c.TariffBracketId)
                    .OnDelete(DeleteBehavior.Restrict);

        foreach (var foreignKey in modelBuilder.Model.GetEntityTypes()
             .SelectMany(e => e.GetForeignKeys()))
        {
            if (foreignKey.DeleteBehavior == DeleteBehavior.Cascade)
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
