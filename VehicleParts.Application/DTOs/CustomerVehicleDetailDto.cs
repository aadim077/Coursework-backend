namespace VehicleParts.Application.DTOs;

public class CustomerVehicleDetailDto
{
    public string CustomerId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public VehicleDto Vehicle { get; set; } = new();
    public List<ServiceHistoryDto> ServiceHistory { get; set; } = new();
}
