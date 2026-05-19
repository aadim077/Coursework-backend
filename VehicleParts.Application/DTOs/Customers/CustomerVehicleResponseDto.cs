namespace VehicleParts.Application.DTOs.Customers;

public class CustomerVehicleResponseDto
{
    public int Id { get; set; }
    public string VehicleNumber { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public int? Year { get; set; }
    public string? Color { get; set; }
}

