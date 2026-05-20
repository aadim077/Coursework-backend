using System;
using System.Collections.Generic;
using System.Text;

using VehicleParts.Domain.Entities;

namespace VehicleParts.Domain.Entities;

public class SalesOrder : BaseEntity
{
    public string CustomerId { get; set; } = string.Empty;
    public AppUser Customer { get; set; } = null!;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public decimal GrossAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal FinalAmount { get; set; }

    public ICollection<SalesOrderItem> Items { get; set; } = new List<SalesOrderItem>();
}