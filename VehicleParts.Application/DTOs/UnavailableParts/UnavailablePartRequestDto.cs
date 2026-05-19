using System.ComponentModel.DataAnnotations;

namespace VehicleParts.Application.DTOs.UnavailableParts;

public class UnavailablePartRequestDto
{
    [Required]
    [StringLength(100)]
    public string PartName { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
}

