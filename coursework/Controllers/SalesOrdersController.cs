using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleParts.Application.DTOs.SalesOrders;
using VehicleParts.Application.Interfaces;

namespace coursework.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]   // any authenticated user (Customer, Staff, Admin)
public class SalesOrdersController : ControllerBase
{
    private readonly ISalesOrderService _salesOrderService;

    public SalesOrdersController(ISalesOrderService salesOrderService)
    {
        _salesOrderService = salesOrderService;
    }

    // POST: api/salesorders
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSalesOrderDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Extract customer ID securely from the JWT token
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(customerId))
            return Unauthorized("Could not identify the customer from the token.");

        var result = await _salesOrderService.CreateAsync(dto, customerId);
        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return CreatedAtAction(nameof(GetMyOrders), result.Data);
    }

    // GET: api/salesorders/my-orders
    [HttpGet("my-orders")]
    public async Task<IActionResult> GetMyOrders()
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(customerId))
            return Unauthorized("Could not identify the customer from the token.");

        var result = await _salesOrderService.GetOrdersByCustomerAsync(customerId);
        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result.Data);
    }
}