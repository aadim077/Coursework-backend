using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleParts.Application.DTOs.Parts;
using VehicleParts.Application.Interfaces;

namespace coursework.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class PartsController : ControllerBase
{
    private readonly IPartService _partService;

    public PartsController(IPartService partService)
    {
        _partService = partService;
    }

    // GET: api/parts
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _partService.GetAllAsync();
        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(result.Data);
    }

    // GET: api/parts/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _partService.GetByIdAsync(id);
        if (!result.IsSuccess)
            return NotFound(result.Message);

        return Ok(result.Data);
    }

    // POST: api/parts
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePartDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _partService.CreateAsync(dto);
        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    // PUT: api/parts/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePartDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _partService.UpdateAsync(id, dto);
        if (!result.IsSuccess)
            return NotFound(result.Message);

        return Ok(result.Data);
    }

    // DELETE: api/parts/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _partService.DeleteAsync(id);
        if (!result.IsSuccess)
            return NotFound(result.Message);

        return Ok(new { message = $"Part {id} deleted successfully." });
    }
}
