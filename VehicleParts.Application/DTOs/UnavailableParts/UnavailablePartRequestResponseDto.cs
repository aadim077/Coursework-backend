namespace VehicleParts.Application.DTOs.UnavailableParts;

public class UnavailablePartRequestResponseDto
{
    public int Id { get; set; }
    public string PartName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

