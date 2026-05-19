using Microsoft.AspNetCore.Mvc;
using VehicleParts.Application.DTOs;
using VehicleParts.Application.Interfaces;

namespace coursework.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SaleInvoicesController : ControllerBase
{
    private readonly ISaleInvoiceService _service;

    public SaleInvoicesController(ISaleInvoiceService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleInvoiceDto dto)
    {
        try
        {
            var invoice = await _service.CreateSaleAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = invoice.SaleInvoiceId }, invoice);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            return Ok(await _service.GetByIdAsync(id));
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpGet("customer/{customerId:int}")]
    public async Task<IActionResult> GetByCustomer(int customerId)
    {
        return Ok(await _service.GetByCustomerIdAsync(customerId));
    }
}