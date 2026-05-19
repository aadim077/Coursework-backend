using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using VehicleParts.Application.Common;
using VehicleParts.Application.DTOs.PurchaseInvoices;
using VehicleParts.Application.Interfaces;
using VehicleParts.Domain.Entities;
using VehicleParts.Infrastructure.Data;

namespace VehicleParts.Infrastructure.Services;

public class PurchaseInvoiceService : IPurchaseInvoiceService
{
    private readonly AppDbContext _context;

    public PurchaseInvoiceService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PurchaseInvoiceResponseDto>> CreateAsync(CreatePurchaseInvoiceDto dto)
    {
        // ── Validate vendor ───────────────────────────────────────────────────
        var vendor = await _context.Vendors.FindAsync(dto.VendorId);
        if (vendor is null)
            return Result<PurchaseInvoiceResponseDto>.Failure($"Vendor with ID {dto.VendorId} not found.");

        // ── Validate all parts exist up-front ─────────────────────────────────
        var partIds = dto.Items.Select(i => i.PartId).Distinct().ToList();
        var parts = await _context.Parts
            .Where(p => partIds.Contains(p.Id))
            .ToListAsync();

        if (parts.Count != partIds.Count)
            return Result<PurchaseInvoiceResponseDto>.Failure("One or more Part IDs are invalid.");

        // ── Wrap everything in a transaction ──────────────────────────────────
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var invoiceItems = new List<PurchaseInvoiceItem>();
            decimal totalAmount = 0;

            foreach (var itemDto in dto.Items)
            {
                var part = parts.First(p => p.Id == itemDto.PartId);
                var subTotal = itemDto.Quantity * itemDto.UnitPrice;

                // Increase stock
                part.StockQuantity += itemDto.Quantity;

                invoiceItems.Add(new PurchaseInvoiceItem
                {
                    PartId = itemDto.PartId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    SubTotal = subTotal
                });

                totalAmount += subTotal;
            }

            var invoice = new PurchaseInvoice
            {
                InvoiceNumber = dto.InvoiceNumber,
                InvoiceDate = dto.InvoiceDate,
                VendorId = dto.VendorId,
                TotalAmount = totalAmount,
                Items = invoiceItems
            };

            _context.PurchaseInvoices.Add(invoice);
            await _context.SaveChangesAsync();   // saves invoice + stock updates atomically
            await transaction.CommitAsync();

            return Result<PurchaseInvoiceResponseDto>.Success(MapToResponse(invoice, vendor, parts));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return Result<PurchaseInvoiceResponseDto>.Failure($"Failed to create invoice: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<PurchaseInvoiceResponseDto>>> GetAllAsync()
    {
        var invoices = await _context.PurchaseInvoices
            .Include(i => i.Vendor)
            .Include(i => i.Items)
                .ThenInclude(item => item.Part)
            .AsNoTracking()
            .ToListAsync();

        var response = invoices.Select(i => MapToResponse(i, i.Vendor, i.Items.Select(x => x.Part).ToList()));
        return Result<IEnumerable<PurchaseInvoiceResponseDto>>.Success(response);
    }

    public async Task<Result<PurchaseInvoiceResponseDto>> GetByIdAsync(int id)
    {
        var invoice = await _context.PurchaseInvoices
            .Include(i => i.Vendor)
            .Include(i => i.Items)
                .ThenInclude(item => item.Part)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice is null)
            return Result<PurchaseInvoiceResponseDto>.Failure($"Invoice with ID {id} not found.");

        return Result<PurchaseInvoiceResponseDto>.Success(
            MapToResponse(invoice, invoice.Vendor, invoice.Items.Select(x => x.Part).ToList()));
    }

    // ── Mapper ────────────────────────────────────────────────────────────────
    private static PurchaseInvoiceResponseDto MapToResponse(
        PurchaseInvoice invoice, Vendor vendor, List<Part> parts) => new()
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            InvoiceDate = invoice.InvoiceDate,
            TotalAmount = invoice.TotalAmount,
            VendorId = invoice.VendorId,
            VendorName = vendor.Name,
            Items = invoice.Items.Select(i => new PurchaseInvoiceItemResponseDto
            {
                PartId = i.PartId,
                PartName = parts.FirstOrDefault(p => p.Id == i.PartId)?.Name ?? string.Empty,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                SubTotal = i.SubTotal
            }).ToList()
        };
}