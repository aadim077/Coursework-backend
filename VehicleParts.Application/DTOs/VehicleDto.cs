namespace VehicleParts.Application.DTOs;

public class VehicleDto
{
    public int Id { get; set; }
    public string VehicleNumber { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Color { get; set; } = string.Empty;
    public string VIN { get; set; } = string.Empty;
    public DateTime RegisteredDate { get; set; }
    public DateTime? LastServiceDate { get; set; }
}
