namespace VehicleParts.Domain.Entities;

public class SalesInvoiceItem : BaseEntity
{
    public int SalesInvoiceId { get; set; }
    public SalesInvoice SalesInvoice { get; set; } = null!;
    
    public int PartId { get; set; }
    public Part Part { get; set; } = null!;
    
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal { get; set; }
}
