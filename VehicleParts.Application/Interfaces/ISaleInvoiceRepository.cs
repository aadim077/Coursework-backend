using VehicleParts.Domain.Entities;

namespace VehicleParts.Application.Interfaces;

public interface ISaleInvoiceRepository : IRepositoryBase<SaleInvoice>
{
    Task<SaleInvoice?> GetByIdWithItemsAsync(int invoiceId);
    Task<IEnumerable<SaleInvoice>> GetByCustomerIdAsync(string customerId);
    Task<string> GenerateInvoiceNumberAsync();
}