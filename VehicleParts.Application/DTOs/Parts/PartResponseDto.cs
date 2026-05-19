using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleParts.Application.DTOs.Parts;

public class PartResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int MinimumStockLevel { get; set; }

    // Vendor info flattened — avoids circular reference in JSON
    public int VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
}
    
