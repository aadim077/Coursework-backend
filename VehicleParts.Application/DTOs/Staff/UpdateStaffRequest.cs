using System.ComponentModel.DataAnnotations;

namespace VehicleParts.Application.DTOs.Staff;

public class UpdateStaffRequest
{
    [StringLength(100, MinimumLength = 2)]
    public string? FullName { get; set; }

    [Phone]
    public string? PhoneNumber { get; set; }

    public string? Role { get; set; }

    public bool? IsActive { get; set; }
}
