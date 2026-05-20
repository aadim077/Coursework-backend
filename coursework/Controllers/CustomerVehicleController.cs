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

    [HttpGet("{customerId}")]
    [Authorize(Policy = "StaffOrAdmin")]
    public async Task<IActionResult> GetCustomerById(string customerId)
    {
        var customer = await _service.GetCustomerByIdAsync(customerId);
        if (customer == null)
            return NotFound(new { message = "Customer not found" });

        return Ok(customer);
    }

    [HttpGet("{customerId}/vehicles/{vehicleId}")]
    [Authorize(Policy = "StaffOrAdmin")]
    public async Task<IActionResult> GetCustomerVehicleDetail(string customerId, int vehicleId)
    {
        var detail = await _service.GetCustomerVehicleDetailAsync(customerId, vehicleId);
        if (detail == null)
            return NotFound(new { message = "Customer or vehicle not found" });

        return Ok(detail);
    }

    [HttpGet("search/vehicle")]
    [Authorize(Policy = "StaffOrAdmin")]
    public async Task<IActionResult> SearchByVehicleNumber([FromQuery] string vehicleNumber)
    {
        if (string.IsNullOrWhiteSpace(vehicleNumber))
            return BadRequest(new { message = "Vehicle number is required" });

        var customers = await _service.SearchCustomersByVehicleNumberAsync(vehicleNumber);
        return Ok(customers);
    }

    [HttpGet("search/phone")]
    [Authorize(Policy = "StaffOrAdmin")]
    public async Task<IActionResult> SearchByPhone([FromQuery] string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return BadRequest(new { message = "Phone number is required" });

        var customers = await _service.SearchCustomersByPhoneAsync(phoneNumber);
        return Ok(customers);
    }

    [HttpGet("search/id")]
    [Authorize(Policy = "StaffOrAdmin")]
    public async Task<IActionResult> SearchById([FromQuery] string customerId)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            return BadRequest(new { message = "Customer ID is required" });

        var customers = await _service.SearchCustomersByIdAsync(customerId);
        return Ok(customers);
    }

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
