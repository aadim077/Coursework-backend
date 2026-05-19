using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VehicleParts.Application.Interfaces;
using VehicleParts.Infrastructure.Data;
using VehicleParts.Infrastructure.Settings;

namespace VehicleParts.Infrastructure.BackgroundJobs;

public class DailyNotificationJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DailyNotificationJob> _logger;
    private readonly EmailSettings _emailSettings;

    public DailyNotificationJob(IServiceProvider serviceProvider, ILogger<DailyNotificationJob> logger, EmailSettings emailSettings)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _emailSettings = emailSettings;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DailyNotificationJob started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessNotificationsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing daily notifications.");
            }

            // For testing, run every 1 minute
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }

        _logger.LogInformation("DailyNotificationJob is stopping.");
    }

    private async Task ProcessNotificationsAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        // 1. Check for Low Stock
        var lowStockParts = await dbContext.Parts
            .Where(p => p.StockQuantity < 10)
            .ToListAsync(stoppingToken);

        if (lowStockParts.Any())
        {
            var subject = "Low Stock Alert!";
            var body = "<h3>The following items are low on stock:</h3><ul>";
            foreach (var part in lowStockParts)
            {
                body += $"<li>{part.Name} (Stock: {part.StockQuantity}, Min Required: {part.MinimumStockLevel})</li>";
            }
            body += "</ul>";

            _logger.LogInformation("Sending low stock alert to Admin.");
            await emailService.SendEmailAsync(_emailSettings.AdminEmail, subject, body);
        }

        // 2. Check for Unpaid Credits > 1 Month
        var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);
        var unpaidInvoices = await dbContext.SalesInvoices
            .Where(i => !i.IsPaid && i.InvoiceDate < oneMonthAgo && !string.IsNullOrEmpty(i.CustomerEmail))
            .ToListAsync(stoppingToken);

        foreach (var invoice in unpaidInvoices)
        {
            var subject = $"Payment Reminder: Invoice {invoice.InvoiceNumber}";
            var body = $@"
                <h3>Payment Reminder</h3>
                <p>Dear {invoice.CustomerName},</p>
                <p>This is a reminder that your invoice <b>{invoice.InvoiceNumber}</b> dated {invoice.InvoiceDate:d} for the amount of <b>${invoice.TotalAmount}</b> is unpaid for more than a month.</p>
                <p>Please clear your dues as soon as possible.</p>
                <br />
                <p>Thank you.</p>";

            _logger.LogInformation("Sending payment reminder to {CustomerEmail} for invoice {InvoiceNumber}.", invoice.CustomerEmail, invoice.InvoiceNumber);
            await emailService.SendEmailAsync(invoice.CustomerEmail!, subject, body);
        }
    }
}
