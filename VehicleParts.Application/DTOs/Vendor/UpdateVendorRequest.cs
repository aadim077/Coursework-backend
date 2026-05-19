using System.ComponentModel.DataAnnotations;

namespace VehicleParts.Application.DTOs.Vendor;

public class UpdateVendorRequest
{
    [StringLength(150, MinimumLength = 2)]
    public string? Name { get; set; }

    [StringLength(100, MinimumLength = 2)]
    public string? ContactPerson { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [Phone]
    public string? Phone { get; set; }

    [StringLength(300, MinimumLength = 5)]
    public string? Address { get; set; }
}
