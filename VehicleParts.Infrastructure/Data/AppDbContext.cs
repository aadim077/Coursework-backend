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
    public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
    public DbSet<SalesOrderItem> SalesOrderItems => Set<SalesOrderItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<SalesOrder>(entity =>
        {
            entity.HasKey(o => o.Id);

            entity.Property(o => o.GrossAmount)
                  .HasColumnType("decimal(18,2)");

            entity.Property(o => o.DiscountAmount)
                  .HasColumnType("decimal(18,2)");

            entity.Property(o => o.FinalAmount)
                  .HasColumnType("decimal(18,2)");

            // Many SalesOrders → one AppUser (Customer)
            entity.HasOne(o => o.Customer)
                  .WithMany()
                  .HasForeignKey(o => o.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);

            // One SalesOrder → many SalesOrderItems
            entity.HasMany(o => o.Items)
                  .WithOne(i => i.SalesOrder)
                  .HasForeignKey(i => i.SalesOrderId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ── SalesOrderItem ─
        builder.Entity<SalesOrderItem>(entity =>
        {
            entity.HasKey(i => i.Id);

            entity.Property(i => i.UnitPrice)
                  .HasColumnType("decimal(18,2)");

            entity.Property(i => i.SubTotal)
                  .HasColumnType("decimal(18,2)");

            // Many SalesOrderItems → one Part
            entity.HasOne(i => i.Part)
                  .WithMany()
                  .HasForeignKey(i => i.PartId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
