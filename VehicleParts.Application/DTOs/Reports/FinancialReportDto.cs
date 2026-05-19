namespace VehicleParts.Application.DTOs.Reports;

public class FinancialReportDto
{
    public DateTime ReportDate { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal NetProfit => TotalIncome - TotalExpense;
    public int NumberOfSales { get; set; }
    public int NumberOfPurchases { get; set; }
}
