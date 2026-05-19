using VehicleParts.Application.DTOs;

namespace VehicleParts.Application.Interfaces;

public interface ISaleInvoiceService
{
    Task<SaleInvoiceDto> CreateSaleAsync(CreateSaleInvoiceDto dto);
    Task<SaleInvoiceDto> GetByIdAsync(int invoiceId);
    Task<IEnumerable<SaleInvoiceDto>> GetByCustomerIdAsync(int customerId);
}