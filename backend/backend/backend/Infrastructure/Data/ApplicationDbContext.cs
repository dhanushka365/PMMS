using Microsoft.EntityFrameworkCore;
using backend.Domain.Entities;
using backend.Domain.Enums;

namespace backend.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MaintenanceRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MaintenanceEventName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PropertyName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Status).HasConversion<int>();
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.UpdatedBy).HasMaxLength(100);
        });

        // Seed data
        modelBuilder.Entity<MaintenanceRequest>().HasData(
            new MaintenanceRequest
            {
                Id = 1,
                MaintenanceEventName = "Fix Leaky Faucet",
                PropertyName = "Apartment 101",
                Description = "Kitchen faucet is leaking and needs immediate attention",
                Status = MaintenanceStatus.New,
                CreatedDate = DateTime.UtcNow.AddDays(-2),
                CreatedBy = "tenant@example.com"
            },
            new MaintenanceRequest
            {
                Id = 2,                MaintenanceEventName = "HVAC System Maintenance",
                PropertyName = "Building A - Common Area",
                Description = "Annual HVAC system maintenance and filter replacement required",
                Status = MaintenanceStatus.Accepted,
                CreatedDate = DateTime.UtcNow.AddDays(-5),
                UpdatedDate = DateTime.UtcNow.AddDays(-1),
                CreatedBy = "manager@example.com",
                UpdatedBy = "admin@example.com"
            },
            new MaintenanceRequest
            {
                Id = 3,
                MaintenanceEventName = "Broken Window",
                PropertyName = "Apartment 205",
                Description = "Bedroom window glass is cracked and needs replacement",
                Status = MaintenanceStatus.New,
                CreatedDate = DateTime.UtcNow.AddHours(-6),
                CreatedBy = "tenant2@example.com"
            }
        );
    }
}
