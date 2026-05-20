using VehicleParts.Application.DTOs;
using VehicleParts.Application.Interfaces;
using VehicleParts.Domain.Entities;
using VehicleParts.Domain.Enums;

namespace VehicleParts.Application.Services;

public class SaleInvoiceService : ISaleInvoiceService
{
    private readonly ISaleInvoiceRepository _invoiceRepo;
    private readonly IPartRepository _partRepo;

    private const decimal LoyaltyThreshold = 5000m;
    private const decimal LoyaltyDiscountRate = 0.10m;
    private const int LowStockThreshold = 10;

    public SaleInvoiceService(
        ISaleInvoiceRepository invoiceRepo,
        IPartRepository partRepo)
    {
        _invoiceRepo = invoiceRepo;
        _partRepo = partRepo;
    }

    public async Task<SaleInvoiceDto> CreateSaleAsync(CreateSaleInvoiceDto dto)
    {
        if (dto.Items == null || !dto.Items.Any())
            throw new ArgumentException("A sale must have at least one item.");

        var invoiceItems = new List<SaleInvoiceItem>();
        decimal subTotal = 0;

        foreach (var itemDto in dto.Items)
        {
            var part = await _partRepo.GetByIdAsync(itemDto.PartId)
                ?? throw new KeyNotFoundException(
                    $"Part with ID {itemDto.PartId} was not found.");

            if (itemDto.Quantity <= 0)
                throw new ArgumentException(
                    $"Quantity for '{part.Name}' must be greater than zero.");

            if (part.StockQuantity < itemDto.Quantity)
                throw new InvalidOperationException(
                    $"Not enough stock for '{part.Name}'. " +
                    $"Available: {part.StockQuantity}, Requested: {itemDto.Quantity}.");

            var lineTotal = part.Price * itemDto.Quantity;
            subTotal += lineTotal;

            invoiceItems.Add(new SaleInvoiceItem
            {
                PartId = part.Id,
                Quantity = itemDto.Quantity,
                UnitPrice = part.Price,
                LineTotal = lineTotal
            });

            part.StockQuantity -= itemDto.Quantity;
            _partRepo.Update(part);
        }

        decimal discountAmount = subTotal > LoyaltyThreshold
            ? Math.Round(subTotal * LoyaltyDiscountRate, 2)
            : 0;

        decimal totalAmount = subTotal - discountAmount;

        var status = dto.PaymentMethod == PaymentMethod.Credit
            ? InvoiceStatus.Credit
            : InvoiceStatus.Paid;

        var invoice = new SaleInvoice
        {
            InvoiceNumber = await _invoiceRepo.GenerateInvoiceNumberAsync(),
            InvoiceDate = DateTime.UtcNow,
            CustomerId = dto.CustomerId,
            StaffId = dto.StaffId,
            PaymentMethod = dto.PaymentMethod,
            Status = status,
            SubTotal = subTotal,
            DiscountAmount = discountAmount,
            TotalAmount = totalAmount,
            Notes = dto.Notes,
            Items = invoiceItems
        };

        _invoiceRepo.Create(invoice);
        await _invoiceRepo.SaveChangesAsync();

        await NotifyLowStockAsync(invoiceItems);

        return await GetByIdAsync(invoice.Id);
    }

    public async Task<SaleInvoiceDto> GetByIdAsync(int invoiceId)
    {
        var invoice = await _invoiceRepo.GetByIdWithItemsAsync(invoiceId)
            ?? throw new KeyNotFoundException($"Invoice ID {invoiceId} not found.");

        return MapToDto(invoice);
    }

    public async Task<IEnumerable<SaleInvoiceDto>> GetByCustomerIdAsync(string customerId)
    {
        var invoices = await _invoiceRepo.GetByCustomerIdAsync(customerId);
        return invoices.Select(MapToDto);
    }

    private static SaleInvoiceDto MapToDto(SaleInvoice inv) => new()
    {
        SaleInvoiceId = inv.Id,
        InvoiceNumber = inv.InvoiceNumber,
        InvoiceDate = inv.InvoiceDate,
        CustomerId = inv.CustomerId,
        CustomerName = inv.Customer != null
        ? inv.Customer.FullName
        : string.Empty,
        StaffId = inv.StaffId,
        StaffName = inv.Staff != null
        ? inv.Staff.FullName
        : string.Empty,
        PaymentMethod = inv.PaymentMethod.ToString(),
        Status = inv.Status.ToString(),
        SubTotal = inv.SubTotal,
        DiscountAmount = inv.DiscountAmount,
        TotalAmount = inv.TotalAmount,
        Notes = inv.Notes,
        Items = inv.Items.Select(i => new SaleInvoiceItemDto
        {
            PartId = i.PartId,
            PartName = i.Part?.Name ?? string.Empty,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
            LineTotal = i.LineTotal
        }).ToList()
    };

    private async Task NotifyLowStockAsync(List<SaleInvoiceItem> items)
    {
        foreach (var item in items)
        {
            var part = await _partRepo.GetByIdAsync(item.PartId);
            if (part != null && part.StockQuantity < LowStockThreshold)
            {
                Console.WriteLine(
                    $"[LOW STOCK WARNING] '{part.Name}' " +
                    $"has only {part.StockQuantity} unit(s) remaining.");
            }
        }
    }
}