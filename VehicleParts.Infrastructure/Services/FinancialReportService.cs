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

        var salesQuery = _context.SalesInvoices
            .Where(s => s.InvoiceDate.Date == targetDate);

        var purchasesQuery = _context.PurchaseInvoices
            .Where(p => p.InvoiceDate.Date == targetDate);

        return new FinancialReportDto
        {
            ReportDate = targetDate,
            TotalIncome = await salesQuery.SumAsync(s => s.TotalAmount),
            TotalExpense = await purchasesQuery.SumAsync(p => p.TotalAmount),
            NumberOfSales = await salesQuery.CountAsync(),
            NumberOfPurchases = await purchasesQuery.CountAsync()
        };
    }

    public async Task<FinancialReportDto> GetMonthlyReportAsync(int year, int month)
    {
        var salesQuery = _context.SalesInvoices
            .Where(s => s.InvoiceDate.Year == year && s.InvoiceDate.Month == month);

        var purchasesQuery = _context.PurchaseInvoices
            .Where(p => p.InvoiceDate.Year == year && p.InvoiceDate.Month == month);

        return new FinancialReportDto
        {
            ReportDate = new DateTime(year, month, 1),
            TotalIncome = await salesQuery.SumAsync(s => s.TotalAmount),
            TotalExpense = await purchasesQuery.SumAsync(p => p.TotalAmount),
            NumberOfSales = await salesQuery.CountAsync(),
            NumberOfPurchases = await purchasesQuery.CountAsync()
        };
    }

    public async Task<FinancialReportDto> GetYearlyReportAsync(int year)
    {
        var salesQuery = _context.SalesInvoices
            .Where(s => s.InvoiceDate.Year == year);

        var purchasesQuery = _context.PurchaseInvoices
            .Where(p => p.InvoiceDate.Year == year);

        return new FinancialReportDto
        {
            ReportDate = new DateTime(year, 1, 1),
            TotalIncome = await salesQuery.SumAsync(s => s.TotalAmount),
            TotalExpense = await purchasesQuery.SumAsync(p => p.TotalAmount),
            NumberOfSales = await salesQuery.CountAsync(),
            NumberOfPurchases = await purchasesQuery.CountAsync()
        };
    }
}
