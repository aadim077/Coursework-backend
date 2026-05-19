namespace VehicleParts.Application.DTOs;

public class ServiceHistoryDto
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime ServiceDate { get; set; }
    public decimal Cost { get; set; }
    public string Notes { get; set; } = string.Empty;
}
