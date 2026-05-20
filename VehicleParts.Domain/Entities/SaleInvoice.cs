using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VehicleParts.Domain.Enums;

namespace VehicleParts.Domain.Entities;

public class SaleInvoice : BaseEntity
{
    [Required]
    [MaxLength(30)]
    public string InvoiceNumber { get; set; } = string.Empty;

    [Required]
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

    [Required]
    public string CustomerId { get; set; } = string.Empty;

    [Required]
    public string StaffId { get; set; } = string.Empty;

    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Paid;

    [Column(TypeName = "decimal(18,2)")]
    public decimal SubTotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    public string? Notes { get; set; }

    [ForeignKey("CustomerId")]
    public AppUser? Customer { get; set; }

    [ForeignKey("StaffId")]
    public AppUser? Staff { get; set; }

    public ICollection<SaleInvoiceItem> Items { get; set; } = new List<SaleInvoiceItem>();
}