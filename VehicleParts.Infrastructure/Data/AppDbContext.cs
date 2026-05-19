using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VehicleParts.Domain.Entities;

namespace VehicleParts.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Part> Parts { get; set; }
    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<PurchaseInvoice> PurchaseInvoices { get; set; }
    public DbSet<PurchaseInvoiceItem> PurchaseInvoiceItems { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<ServiceHistory> ServiceHistories { get; set; }

    public DbSet<CustomerVehicle> CustomerVehicles { get; set; }

    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<UnavailablePartRequest> UnavailablePartRequests { get; set; }
    public DbSet<Review> Reviews { get; set; }

    public DbSet<CustomerPurchase> CustomerPurchases { get; set; }
    public DbSet<CustomerPurchaseItem> CustomerPurchaseItems { get; set; }

    public DbSet<SaleInvoice> SaleInvoices { get; set; }
    public DbSet<SaleInvoiceItem> SaleInvoiceItems { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // CustomerVehicle configuration
        builder.Entity<CustomerVehicle>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.VehicleNumber)
                .IsRequired()
                .HasMaxLength(32);

            entity.Property(e => e.Make)
                .IsRequired()
                .HasMaxLength(32);

            entity.Property(e => e.Model)
                .IsRequired()
                .HasMaxLength(32);

            entity.Property(e => e.Color)
                .HasMaxLength(32);

            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.VehicleNumber).IsUnique();

            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Appointment configuration
        builder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.CustomerId).IsRequired();
            entity.Property(e => e.VehicleId).IsRequired();
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.AppointmentDateTime).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Vehicle)
                .WithMany()
                .HasForeignKey(e => e.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // UnavailablePartRequest configuration
        builder.Entity<UnavailablePartRequest>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.CustomerId).IsRequired();
            entity.Property(e => e.PartName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Review configuration
        builder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.CustomerId).IsRequired();
            entity.Property(e => e.Rating).IsRequired();
            entity.Property(e => e.Comment).IsRequired().HasMaxLength(500);
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // CustomerPurchase configuration
        builder.Entity<CustomerPurchase>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.CustomerId).IsRequired();

            entity.Property(e => e.PurchaseDate)
                .IsRequired();

            entity.Property(e => e.ReferenceNumber)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.TotalAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(32);

            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.ReferenceNumber).IsUnique();

            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Items)
                .WithOne(e => e.CustomerPurchase)
                .HasForeignKey(e => e.CustomerPurchaseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CustomerPurchaseItem configuration
        builder.Entity<CustomerPurchaseItem>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.CustomerPurchaseId).IsRequired();

            entity.Property(e => e.ItemName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Quantity)
                .IsRequired();

            entity.Property(e => e.UnitPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(e => e.LineTotal)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasIndex(e => e.CustomerPurchaseId);
            entity.HasIndex(e => e.PartId);

            entity.HasOne(e => e.Part)
                .WithMany()
                .HasForeignKey(e => e.PartId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}