namespace VehicleParts.Application.DTOs.Reports;

public class FinancialReportDto
{
    public DateTime ReportDate { get; set; } // Can represent the Day, Month start, or Year start
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal NetProfit => TotalIncome - TotalExpense;
    
    // Additional metrics
    public int NumberOfSales { get; set; }
    public int NumberOfPurchases { get; set; }
}
