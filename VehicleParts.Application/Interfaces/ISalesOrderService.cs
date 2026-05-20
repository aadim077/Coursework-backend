using System;
using System.Collections.Generic;
using System.Text;

using VehicleParts.Application.Common;
using VehicleParts.Application.DTOs.SalesOrders;

namespace VehicleParts.Application.Interfaces;

public interface ISalesOrderService
{
    Task<Result<SalesOrderResponseDto>> CreateAsync(CreateSalesOrderDto dto, string customerId);
    Task<Result<IEnumerable<SalesOrderResponseDto>>> GetOrdersByCustomerAsync(string customerId);
}