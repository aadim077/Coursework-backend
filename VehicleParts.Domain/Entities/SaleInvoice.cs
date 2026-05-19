using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VehicleParts.Domain.Enums;

namespace VehicleParts.Domain.Entities;

public class SaleInvoice : BaseEntity
{
    [Required]
    [MaxLength(30)]
    public string InvoiceNumber { get; set; } = string.Empty; // e.g. INV-20260427-001

    [Required]
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

    // The customer who bought (AppUser with role Customer)
    [Required]
    public int CustomerId { get; set; }

    // The staff who processed the sale (AppUser with role Staff)
    [Required]
    public int StaffId { get; set; }

    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Paid;

    [Column(TypeName = "decimal(18,2)")]
    public decimal SubTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; } = 0;   // 10% loyalty if SubTotal > 5000

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    public string? Notes { get; set; }

    // Navigation properties
    [ForeignKey("CustomerId")]
    public AppUser? Customer { get; set; }

    [ForeignKey("StaffId")]
    public AppUser? Staff { get; set; }

    public ICollection<SaleInvoiceItem> Items { get; set; } = new List<SaleInvoiceItem>();
}