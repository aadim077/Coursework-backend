using System.ComponentModel.DataAnnotations.Schema;
using VehicleParts.Domain.Enums;

namespace VehicleParts.Domain.Entities;

public class Appointment : BaseEntity
{
    public string CustomerId { get; set; } = string.Empty;
    public AppUser Customer { get; set; } = null!;
    public int VehicleId { get; set; }
    public CustomerVehicle Vehicle { get; set; } = null!;
    public DateTime AppointmentDateTime { get; set; }
    public string Description { get; set; } = string.Empty;
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
}

