namespace VehicleParts.Domain.Entities;

public class Vehicle : BaseEntity
{
    public string VehicleNumber { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Color { get; set; } = string.Empty;
    public string VIN { get; set; } = string.Empty;

    public string CustomerId { get; set; } = string.Empty;
    public AppUser Customer { get; set; } = null!;

    public DateTime RegisteredDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastServiceDate { get; set; }

    public ICollection<ServiceHistory> ServiceHistories { get; set; } = new List<ServiceHistory>();
}
