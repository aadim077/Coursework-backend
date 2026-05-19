using System.ComponentModel.DataAnnotations;

namespace VehicleParts.Application.DTOs.Customers;

public class CustomerVehicleRequestDto
{
    [Required]
    public string VehicleNumber { get; set; }

    [Required]
    public string Make { get; set; }

    [Required]
    public string Model { get; set; }

    public int? Year { get; set; }
    public string? Color { get; set; }
}

