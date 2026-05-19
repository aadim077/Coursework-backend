using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleParts.Domain.Entities;

public class CustomerVehicle : BaseEntity
{
    public string CustomerId { get; set; } // Foreign key to AppUser
    public AppUser Customer { get; set; }  // Navigation property
    public string VehicleNumber { get; set; } // Required
    public string Make { get; set; } // Required
    public string Model { get; set; } // Required
    public int? Year { get; set; } // Optional
    public string? Color { get; set; } // Optional
}

