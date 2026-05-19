using System.ComponentModel.DataAnnotations;

namespace VehicleParts.Application.DTOs.Staff;

public class UpdateStaffPasswordRequest
{
    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string NewPassword { get; set; } = string.Empty;
}
