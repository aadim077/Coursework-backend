using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleParts.Application.DTOs.Vendor;
using VehicleParts.Application.Interfaces;

namespace coursework.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "StaffOrAdmin")]
public class VendorController : ControllerBase
{
    private readonly IVendorService _vendorService;

    public VendorController(IVendorService vendorService)
    {
        _vendorService = vendorService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllVendors(CancellationToken cancellationToken)
    {
        var result = await _vendorService.GetAllVendorsAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetVendorById(int id, CancellationToken cancellationToken)
    {
        var result = await _vendorService.GetVendorByIdAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateVendor([FromBody] CreateVendorRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _vendorService.CreateVendorAsync(request, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetVendorById), new { id = result.Data!.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateVendor(int id, [FromBody] UpdateVendorRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _vendorService.UpdateVendorAsync(id, request, cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteVendor(int id, CancellationToken cancellationToken)
    {
        var result = await _vendorService.DeleteVendorAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }
}
