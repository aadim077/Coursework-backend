using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleParts.Domain.Entities;

public class SaleInvoiceItem : BaseEntity
{
    [Required]
    public int SaleInvoiceId { get; set; }

    [Required]
    public int PartId { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }    // Captured at time of sale

    [Column(TypeName = "decimal(18,2)")]
    public decimal LineTotal { get; set; }    // UnitPrice * Quantity

    // Navigation
    [ForeignKey("SaleInvoiceId")]
    public SaleInvoice? SaleInvoice { get; set; }

    [ForeignKey("PartId")]
    public Part? Part { get; set; }
}