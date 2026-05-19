using Microsoft.EntityFrameworkCore;
using VehicleParts.Application.Interfaces;
using VehicleParts.Domain.Entities;
using VehicleParts.Infrastructure.Data;

namespace VehicleParts.Infrastructure.Repositories;

public class SaleInvoiceRepository : RepositoryBase<SaleInvoice>, ISaleInvoiceRepository
{
    private readonly AppDbContext _context;

    public SaleInvoiceRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<SaleInvoice?> GetByIdWithItemsAsync(int invoiceId)
    {
        return await _context.SaleInvoices
            .Include(i => i.Customer)
            .Include(i => i.Staff)
            .Include(i => i.Items)
                .ThenInclude(item => item.Part)
            .FirstOrDefaultAsync(i => i.Id == invoiceId);
    }

    public async Task<IEnumerable<SaleInvoice>> GetByCustomerIdAsync(int customerId)
    {
        return await _context.SaleInvoices
            .Include(i => i.Items)
                .ThenInclude(item => item.Part)
            .Where(i => i.CustomerId == customerId)
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync();
    }

    // Auto-generates: INV-20260427-001, INV-20260427-002, etc.
    public async Task<string> GenerateInvoiceNumberAsync()
    {
        var today = DateTime.UtcNow.ToString("yyyyMMdd");
        var prefix = $"INV-{today}-";

        var lastInvoice = await _context.SaleInvoices
            .Where(i => i.InvoiceNumber.StartsWith(prefix))
            .OrderByDescending(i => i.InvoiceNumber)
            .FirstOrDefaultAsync();

        int next = 1;
        if (lastInvoice != null)
        {
            var lastPart = lastInvoice.InvoiceNumber.Split('-').Last();
            if (int.TryParse(lastPart, out int lastNum))
                next = lastNum + 1;
        }

        return $"{prefix}{next:D3}";
    }
}