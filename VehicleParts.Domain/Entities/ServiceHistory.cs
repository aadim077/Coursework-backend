namespace VehicleParts.Domain.Entities;

public class ServiceHistory : BaseEntity
{
    public string Description { get; set; } = string.Empty;
    public DateTime ServiceDate { get; set; } = DateTime.UtcNow;
    public decimal Cost { get; set; }
    public string Notes { get; set; } = string.Empty;

    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;
}
