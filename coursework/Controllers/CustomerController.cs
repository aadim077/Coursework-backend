using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleParts.Application.DTOs.Customers;
using VehicleParts.Application.Interfaces;

namespace coursework.Controllers;

[ApiController]
[Route("api/customer")]
[Authorize(Policy = "CustomerOnly")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerProfileService _service;
    private readonly IAppointmentService _appointmentService;
    private readonly IUnavailablePartRequestService _unavailablePartRequestService;
    private readonly IReviewService _reviewService;
    private readonly ICustomerHistoryService _customerHistoryService;

    public CustomerController(
        ICustomerProfileService service,
        IAppointmentService appointmentService,
        IUnavailablePartRequestService unavailablePartRequestService,
        IReviewService reviewService,
        ICustomerHistoryService customerHistoryService)
    {
        _service = service;
        _appointmentService = appointmentService;
        _unavailablePartRequestService = unavailablePartRequestService;
        _reviewService = reviewService;
        _customerHistoryService = customerHistoryService;
    }

    private string? GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var result = await _service.GetProfileAsync(userId);
        if (!result.IsSuccess)
            return NotFound(result.Message);

        return Ok(result.Data);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateCustomerProfileRequestDto dto)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var result = await _service.UpdateProfileAsync(userId, dto.FullName, dto.PhoneNumber);
        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok();
    }

    [HttpGet("vehicles")]
    public async Task<IActionResult> GetVehicles()
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var result = await _service.GetVehiclesAsync(userId);
        return Ok(result.Data);
    }

    [HttpGet("vehicles/{id}")]
    public async Task<IActionResult> GetVehicle(int id)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var result = await _service.GetVehicleByIdAsync(userId, id);
        if (!result.IsSuccess)
            return NotFound(result.Message);

        return Ok(result.Data);
    }

    [HttpPost("vehicles")]
    public async Task<IActionResult> CreateVehicle([FromBody] CustomerVehicleRequestDto dto)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var result = await _service.CreateVehicleAsync(userId, dto);
        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok();
    }

    [HttpPut("vehicles/{id}")]
    public async Task<IActionResult> UpdateVehicle(int id, [FromBody] CustomerVehicleRequestDto dto)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var result = await _service.UpdateVehicleAsync(userId, id, dto);
        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok();
    }

    [HttpDelete("vehicles/{id}")]
    public async Task<IActionResult> DeleteVehicle(int id)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var result = await _service.DeleteVehicleAsync(userId, id);
        if (!result.IsSuccess)
            return NotFound(result.Message);

        return NoContent();
    }

    // 1. POST customer appointment booking
    [HttpPost("appointments")]
    public async Task<IActionResult> BookAppointment([FromBody] VehicleParts.Application.DTOs.Appointments.AppointmentRequestDto dto)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var result = await _appointmentService.BookAppointmentAsync(userId, dto);
        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result.Data);
    }

    // 2. GET customer appointments
    [HttpGet("appointments")]
    public async Task<IActionResult> GetAppointments()
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var result = await _appointmentService.GetAppointmentsAsync(userId);
        return Ok(result.Data);
    }

    // 3. POST unavailable part request
    [HttpPost("unavailable-parts")]
    public async Task<IActionResult> RequestUnavailablePart([FromBody] VehicleParts.Application.DTOs.UnavailableParts.UnavailablePartRequestDto dto)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var result = await _unavailablePartRequestService.RequestUnavailablePartAsync(userId, dto);
        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result.Data);
    }

    // 4. GET unavailable part requests
    [HttpGet("unavailable-parts")]
    public async Task<IActionResult> GetUnavailablePartRequests()
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var result = await _unavailablePartRequestService.GetUnavailablePartRequestsAsync(userId);
        return Ok(result.Data);
    }

    // 5. POST review submission
    [HttpPost("reviews")]
    public async Task<IActionResult> SubmitReview([FromBody] VehicleParts.Application.DTOs.Reviews.ReviewRequestDto dto)
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var result = await _reviewService.SubmitReviewAsync(userId, dto);
        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result.Data);
    }

    // 6. GET reviews
    [HttpGet("reviews")]
    public async Task<IActionResult> GetReviews()
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var result = await _reviewService.GetReviewsAsync(userId);
        return Ok(result.Data);
    }

    [HttpGet("service-history")]
    public async Task<IActionResult> GetServiceHistory()
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var result = await _customerHistoryService.GetServiceHistoryAsync(userId);
        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result.Data);
    }

    [HttpGet("purchase-history")]
    public async Task<IActionResult> GetPurchaseHistory()
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var result = await _customerHistoryService.GetPurchaseHistoryAsync(userId);
        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result.Data);
    }
}