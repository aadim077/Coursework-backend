using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleParts.Domain.Entities;

public class SalesOrderItem : BaseEntity
{
    public int SalesOrderId { get; set; }           // FK → SalesOrder
    public SalesOrder SalesOrder { get; set; } = null!;

    public int PartId { get; set; }                 // FK → Part
    public Part Part { get; set; } = null!;

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    /// <summary>Quantity × UnitPrice — computed and stored for reporting convenience.</summary>
    public decimal SubTotal { get; set; }
}
