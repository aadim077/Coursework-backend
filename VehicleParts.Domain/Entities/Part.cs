namespace VehicleParts.Domain.Entities;

public class Part : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    
    // Low stock threshold
    public int MinimumStockLevel { get; set; } = 10;
    
    // Foreign Key for Vendor
    public int VendorId { get; set; }
    public Vendor Vendor { get; set; } = null!;
}
