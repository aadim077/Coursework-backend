using Microsoft.EntityFrameworkCore;
using VehicleParts.Application.DTOs.Reports;
using VehicleParts.Application.Interfaces;
using VehicleParts.Infrastructure.Data;

namespace VehicleParts.Infrastructure.Services;

public class FinancialReportService : IFinancialReportService
{
    private readonly AppDbContext _context;

    public FinancialReportService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<FinancialReportDto> GetDailyReportAsync(DateTime date)
    {
        var targetDate = date.Date;

        var sales = await _context.SalesInvoices
            .Where(s => s.InvoiceDate.Date == targetDate)
            .ToListAsync();

        var purchases = await _context.PurchaseInvoices
            .Where(p => p.InvoiceDate.Date == targetDate)
            .ToListAsync();

        return new FinancialReportDto
        {
            ReportDate = targetDate,
            TotalIncome = sales.Sum(s => s.TotalAmount),
            TotalExpense = purchases.Sum(p => p.TotalAmount),
            NumberOfSales = sales.Count,
            NumberOfPurchases = purchases.Count
        };
    }

    public async Task<FinancialReportDto> GetMonthlyReportAsync(int year, int month)
    {
        var sales = await _context.SalesInvoices
            .Where(s => s.InvoiceDate.Year == year && s.InvoiceDate.Month == month)
            .ToListAsync();

        var purchases = await _context.PurchaseInvoices
            .Where(p => p.InvoiceDate.Year == year && p.InvoiceDate.Month == month)
            .ToListAsync();

        return new FinancialReportDto
        {
            ReportDate = new DateTime(year, month, 1),
            TotalIncome = sales.Sum(s => s.TotalAmount),
            TotalExpense = purchases.Sum(p => p.TotalAmount),
            NumberOfSales = sales.Count,
            NumberOfPurchases = purchases.Count
        };
    }

    public async Task<FinancialReportDto> GetYearlyReportAsync(int year)
    {
        var sales = await _context.SalesInvoices
            .Where(s => s.InvoiceDate.Year == year)
            .ToListAsync();

        var purchases = await _context.PurchaseInvoices
            .Where(p => p.InvoiceDate.Year == year)
            .ToListAsync();

        return new FinancialReportDto
        {
            ReportDate = new DateTime(year, 1, 1),
            TotalIncome = sales.Sum(s => s.TotalAmount),
            TotalExpense = purchases.Sum(p => p.TotalAmount),
            NumberOfSales = sales.Count,
            NumberOfPurchases = purchases.Count
        };
    }
}
