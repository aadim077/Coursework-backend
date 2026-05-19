using VehicleParts.Domain.Enums;

namespace VehicleParts.Domain.Entities;

public class UnavailablePartRequest : BaseEntity
{
    public string CustomerId { get; set; } = string.Empty;
    public AppUser Customer { get; set; } = null!;
    public string PartName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public UnavailablePartRequestStatus Status { get; set; } = UnavailablePartRequestStatus.Pending;
}

