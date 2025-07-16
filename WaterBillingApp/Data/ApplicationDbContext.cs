using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WaterBillingApp.Data.Entities;

/// <summary>
/// The database context for the Water Billing application, 
/// extending IdentityDbContext to include ASP.NET Identity support.
/// Configures DbSets and entity relationships.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets representing tables in the database
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Meter> Meters { get; set; }
    public DbSet<Consumption> Consumptions { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<TariffBracket> TariffBrackets { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<MeterRequest> MeterRequests { get; set; }

    /// <summary>
    /// Configure model properties and relationships using Fluent API.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure precision and optionality for TariffBracket properties
        modelBuilder.Entity<TariffBracket>()
            .Property(t => t.MaxVolume)
            .HasPrecision(10, 3)  // Up to 10 digits, 3 decimal places
            .IsRequired(false);   // MaxVolume is optional

        modelBuilder.Entity<TariffBracket>()
            .Property(t => t.MinVolume)
            .HasPrecision(10, 3);

        modelBuilder.Entity<TariffBracket>()
            .Property(t => t.PricePerCubicMeter)
            .HasPrecision(10, 4);  // Price precision with 4 decimals

        // One-to-one relationship between ApplicationUser and Customer,
        // with restricted delete behavior to prevent cascading deletes
        modelBuilder.Entity<ApplicationUser>()
            .HasOne(u => u.Customer)
            .WithOne(c => c.ApplicationUser)
            .HasForeignKey<Customer>(c => c.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // One-to-many relationship between Customer and Meters,
        // with restricted delete behavior
        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Meters)
            .WithOne(m => m.Customer)
            .HasForeignKey(m => m.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure precision for Invoice.TotalAmount
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.Property(e => e.TotalAmount)
                .HasPrecision(18, 2);
        });

        // Configure precision for Consumption.Volume
        modelBuilder.Entity<Consumption>(entity =>
        {
            entity.Property(c => c.Volume)
                .HasPrecision(18, 2);
        });

        // Configure relationship between Consumption and TariffBracket
        modelBuilder.Entity<Consumption>()
            .HasOne(c => c.TariffBracket)
            .WithMany()
            .HasForeignKey(c => c.TariffBracketId)
            .OnDelete(DeleteBehavior.Restrict);

        // Set all cascade delete behaviors to restrict globally to prevent accidental deletes
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
