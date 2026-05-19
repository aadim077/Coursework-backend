using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleParts.Application.DTOs.Parts;

public class UpdatePartDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int MinimumStockLevel { get; set; }
    public int VendorId { get; set; }
}