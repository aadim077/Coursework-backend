namespace VehicleParts.Domain.Entities;

public class SalesInvoice : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    
    // In a real system, there might be a Customer entity, but we will omit it for simplicity if not requested.
    // Or we could add CustomerName
    public string CustomerName { get; set; } = string.Empty;
    
    public ICollection<SalesInvoiceItem> Items { get; set; } = new List<SalesInvoiceItem>();
}
