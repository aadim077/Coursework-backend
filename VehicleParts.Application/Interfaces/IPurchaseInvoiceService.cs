using System;
using System.Collections.Generic;
using System.Text;

using VehicleParts.Application.Common;
using VehicleParts.Application.DTOs.PurchaseInvoices;

namespace VehicleParts.Application.Interfaces;

public interface IPurchaseInvoiceService
{
    Task<Result<PurchaseInvoiceResponseDto>> CreateAsync(CreatePurchaseInvoiceDto dto);
    Task<Result<IEnumerable<PurchaseInvoiceResponseDto>>> GetAllAsync();
    Task<Result<PurchaseInvoiceResponseDto>> GetByIdAsync(int id);
}