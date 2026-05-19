using System.ComponentModel.DataAnnotations;

namespace VehicleParts.Application.DTOs.Appointments;

public class AppointmentRequestDto
{
    [Required]
    public int VehicleId { get; set; }

    [Required]
    public DateTime AppointmentDateTime { get; set; }

    [Required]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
}

