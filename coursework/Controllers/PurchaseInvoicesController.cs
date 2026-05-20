using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleParts.Application.DTOs.PurchaseInvoices;
using VehicleParts.Application.Interfaces;

namespace coursework.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class PurchaseInvoicesController : ControllerBase
{
    private readonly IPurchaseInvoiceService _invoiceService;

    public PurchaseInvoicesController(IPurchaseInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _invoiceService.GetAllAsync();
        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _invoiceService.GetByIdAsync(id);
        if (!result.IsSuccess)
            return NotFound(result.Message);

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseInvoiceDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _invoiceService.CreateAsync(dto);
        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }
}