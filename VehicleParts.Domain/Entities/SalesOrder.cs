using System;
using System.Collections.Generic;
using System.Text;

using VehicleParts.Domain.Entities;

namespace VehicleParts.Domain.Entities;

public class SalesOrder : BaseEntity
{
    public string CustomerId { get; set; } = string.Empty;      // FK → AppUser.Id (string, IdentityUser)
    public AppUser Customer { get; set; } = null!;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    /// <summary>Sum of (UnitPrice × Quantity) across all items — before discount.</summary>
    public decimal GrossAmount { get; set; }

    /// <summary>10 % off GrossAmount when GrossAmount > 5000, otherwise 0.</summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>GrossAmount − DiscountAmount.</summary>
    public decimal FinalAmount { get; set; }

    public ICollection<SalesOrderItem> Items { get; set; } = new List<SalesOrderItem>();
}