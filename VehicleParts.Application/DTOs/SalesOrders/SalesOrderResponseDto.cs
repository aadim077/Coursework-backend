using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleParts.Application.DTOs.SalesOrders;

public class SalesOrderResponseDto
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public bool LoyaltyDiscountApplied { get; set; }    // handy for the UI
    public List<SalesOrderItemResponseDto> Items { get; set; } = new();
}

public class SalesOrderItemResponseDto
{
    public int PartId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal { get; set; }
}