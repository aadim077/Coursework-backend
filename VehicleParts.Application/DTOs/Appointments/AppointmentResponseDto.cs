namespace VehicleParts.Application.DTOs.Appointments;

public class AppointmentResponseDto
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public DateTime AppointmentDateTime { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

