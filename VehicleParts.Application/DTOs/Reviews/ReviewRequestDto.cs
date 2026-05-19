using System.ComponentModel.DataAnnotations;

namespace VehicleParts.Application.DTOs.Reviews;

public class ReviewRequestDto
{
    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }

    [Required]
    [StringLength(500)]
    public string Comment { get; set; } = string.Empty;
}

