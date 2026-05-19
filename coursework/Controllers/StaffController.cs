using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleParts.Application.DTOs.Staff;
using VehicleParts.Application.Interfaces;

namespace coursework.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class StaffController : ControllerBase
{
    private readonly IStaffService _staffService;

    public StaffController(IStaffService staffService)
    {
        _staffService = staffService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllStaff(CancellationToken cancellationToken)
    {
        var result = await _staffService.GetAllStaffAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetStaffById(string id, CancellationToken cancellationToken)
    {
        var result = await _staffService.GetStaffByIdAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateStaff([FromBody] CreateStaffRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _staffService.CreateStaffAsync(request, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetStaffById), new { id = result.Data!.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStaff(string id, [FromBody] UpdateStaffRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _staffService.UpdateStaffAsync(id, request, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut("{id}/password")]
    public async Task<IActionResult> ResetStaffPassword(string id, [FromBody] UpdateStaffPasswordRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _staffService.ResetStaffPasswordAsync(id, request, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStaff(string id, CancellationToken cancellationToken)
    {
        var result = await _staffService.DeleteStaffAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }
}
