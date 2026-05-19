using System.ComponentModel.DataAnnotations;

namespace VehicleParts.Application.DTOs.Vendor;

public class CreateVendorRequest
{
    [Required]
    [StringLength(150, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string ContactPerson { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [StringLength(300, MinimumLength = 5)]
    public string Address { get; set; } = string.Empty;
}
