using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleParts.Application.Interfaces;

namespace coursework.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin,Staff")]
public class FinancialReportsController : ControllerBase
{
    private readonly IFinancialReportService _financialReportService;

    public FinancialReportsController(IFinancialReportService financialReportService)
    {
        _financialReportService = financialReportService;
    }

    [HttpGet("daily")]
    public async Task<IActionResult> GetDailyReport([FromQuery] DateTime date)
    {
        // If no date is provided, it defaults to DateTime.MinValue. 
        // We can default to today if it's the minimum value
        if (date == default)
        {
            date = DateTime.UtcNow;
        }

        var report = await _financialReportService.GetDailyReportAsync(date);
        return Ok(report);
    }

    [HttpGet("monthly")]
    public async Task<IActionResult> GetMonthlyReport([FromQuery] int year, [FromQuery] int month)
    {
        if (year <= 0 || month < 1 || month > 12)
        {
            return BadRequest("Invalid year or month.");
        }

        var report = await _financialReportService.GetMonthlyReportAsync(year, month);
        return Ok(report);
    }

    [HttpGet("yearly")]
    public async Task<IActionResult> GetYearlyReport([FromQuery] int year)
    {
        if (year <= 0)
        {
            return BadRequest("Invalid year.");
        }

        var report = await _financialReportService.GetYearlyReportAsync(year);
        return Ok(report);
    }
}
