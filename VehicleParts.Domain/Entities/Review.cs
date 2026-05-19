namespace VehicleParts.Domain.Entities;

public class Review : BaseEntity
{
    public string CustomerId { get; set; } = string.Empty;
    public AppUser Customer { get; set; } = null!;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}

