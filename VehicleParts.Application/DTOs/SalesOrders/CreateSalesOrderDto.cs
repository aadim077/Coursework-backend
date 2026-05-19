using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleParts.Application.DTOs.SalesOrders;

public class CreateSalesOrderDto
{
    public List<CreateSalesOrderItemDto> Items { get; set; } = new();
}

public class CreateSalesOrderItemDto
{
    public int PartId { get; set; }
    public int Quantity { get; set; }
}