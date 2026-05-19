using VehicleParts.Application.DTOs.Reports;

namespace VehicleParts.Application.Interfaces;

public interface IFinancialReportService
{
    Task<FinancialReportDto> GetDailyReportAsync(DateTime date);
    Task<FinancialReportDto> GetMonthlyReportAsync(int year, int month);
    Task<FinancialReportDto> GetYearlyReportAsync(int year);
}
