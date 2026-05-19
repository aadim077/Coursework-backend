using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleParts.Application.DTOs;
using VehicleParts.Application.Interfaces;

namespace coursework.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomerVehicleController : ControllerBase
{
    private readonly ICustomerVehicleService _service;

    public CustomerVehicleController(ICustomerVehicleService service)
    {
        _service = service;
    }

    /// <summary>
    /// Register a new customer with vehicle details (Staff only)
    /// </summary>
    [HttpPost("register")]
    [Authorize(Policy = "StaffOrAdmin")]
    public async Task<IActionResult> RegisterCustomerWithVehicle([FromBody] RegisterCustomerWithVehicleDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, message, customerId) = await _service.RegisterCustomerWithVehicleAsync(dto);
        if (!success)
            return BadRequest(new { message });

        return Ok(new { message, customerId });
    }

    /// <summary>
    /// Get customer details by customer ID (Staff only)
    /// </summary>
    [HttpGet("{customerId}")]
    [Authorize(Policy = "StaffOrAdmin")]
    public async Task<IActionResult> GetCustomerById(string customerId)
    {
        var customer = await _service.GetCustomerByIdAsync(customerId);
        if (customer == null)
            return NotFound(new { message = "Customer not found" });

        return Ok(customer);
    }

    /// <summary>
    /// Get customer and vehicle details with service history (Staff only)
    /// </summary>
    [HttpGet("{customerId}/vehicles/{vehicleId}")]
    [Authorize(Policy = "StaffOrAdmin")]
    public async Task<IActionResult> GetCustomerVehicleDetail(string customerId, int vehicleId)
    {
        var detail = await _service.GetCustomerVehicleDetailAsync(customerId, vehicleId);
        if (detail == null)
            return NotFound(new { message = "Customer or vehicle not found" });

        return Ok(detail);
    }

    /// <summary>
    /// Search customers by vehicle number (Staff only)
    /// </summary>
    [HttpGet("search/vehicle")]
    [Authorize(Policy = "StaffOrAdmin")]
    public async Task<IActionResult> SearchByVehicleNumber([FromQuery] string vehicleNumber)
    {
        if (string.IsNullOrWhiteSpace(vehicleNumber))
            return BadRequest(new { message = "Vehicle number is required" });

        var customers = await _service.SearchCustomersByVehicleNumberAsync(vehicleNumber);
        return Ok(customers);
    }

    /// <summary>
    /// Search customers by phone number (Staff only)
    /// </summary>
    [HttpGet("search/phone")]
    [Authorize(Policy = "StaffOrAdmin")]
    public async Task<IActionResult> SearchByPhone([FromQuery] string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return BadRequest(new { message = "Phone number is required" });

        var customers = await _service.SearchCustomersByPhoneAsync(phoneNumber);
        return Ok(customers);
    }

    /// <summary>
    /// Search customers by ID (Staff only)
    /// </summary>
    [HttpGet("search/id")]
    [Authorize(Policy = "StaffOrAdmin")]
    public async Task<IActionResult> SearchById([FromQuery] string customerId)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            return BadRequest(new { message = "Customer ID is required" });

        var customers = await _service.SearchCustomersByIdAsync(customerId);
        return Ok(customers);
    }

    /// <summary>
    /// Search customers by name (Staff only)
    /// </summary>
    [HttpGet("search/name")]
    [Authorize(Policy = "StaffOrAdmin")]
    public async Task<IActionResult> SearchByName([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest(new { message = "Customer name is required" });

        var customers = await _service.SearchCustomersByNameAsync(name);
        return Ok(customers);
    }
}
