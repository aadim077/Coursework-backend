using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using VehicleParts.Application.Common;
using VehicleParts.Application.DTOs.SalesOrders;
using VehicleParts.Application.Interfaces;
using VehicleParts.Domain.Entities;
using VehicleParts.Infrastructure.Data;

namespace VehicleParts.Infrastructure.Services;

public class SalesOrderService : ISalesOrderService
{
    private readonly AppDbContext _context;
    private const decimal LoyaltyThreshold = 5000m;
    private const decimal LoyaltyDiscountRate = 0.10m;

    public SalesOrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SalesOrderResponseDto>> CreateAsync(CreateSalesOrderDto dto, string customerId)
    {
        var customerExists = await _context.Users.AnyAsync(u => u.Id == customerId);
        if (!customerExists)
            return Result<SalesOrderResponseDto>.Failure("Customer not found.");

        var partIds = dto.Items.Select(i => i.PartId).Distinct().ToList();
        var parts = await _context.Parts
            .Where(p => partIds.Contains(p.Id))
            .ToListAsync();

        if (parts.Count != partIds.Count)
            return Result<SalesOrderResponseDto>.Failure("One or more Part IDs are invalid.");

        foreach (var itemDto in dto.Items)
        {
            var part = parts.First(p => p.Id == itemDto.PartId);
            if (part.StockQuantity < itemDto.Quantity)
                return Result<SalesOrderResponseDto>.Failure(
                    $"Insufficient stock for '{part.Name}'. Available: {part.StockQuantity}, Requested: {itemDto.Quantity}.");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var orderItems = new List<SalesOrderItem>();
            decimal grossAmount = 0;

            foreach (var itemDto in dto.Items)
            {
                var part = parts.First(p => p.Id == itemDto.PartId);
                var subTotal = itemDto.Quantity * part.Price;

                part.StockQuantity -= itemDto.Quantity;

                orderItems.Add(new SalesOrderItem
                {
                    PartId = part.Id,
                    Quantity = itemDto.Quantity,
                    UnitPrice = part.Price,
                    SubTotal = subTotal
                });

                grossAmount += subTotal;
            }

            decimal discountAmount = grossAmount > LoyaltyThreshold
                ? Math.Round(grossAmount * LoyaltyDiscountRate, 2)
                : 0m;

            decimal finalAmount = grossAmount - discountAmount;

            var order = new SalesOrder
            {
                CustomerId = customerId,
                OrderDate = DateTime.UtcNow,
                GrossAmount = grossAmount,
                DiscountAmount = discountAmount,
                FinalAmount = finalAmount,
                Items = orderItems
            };

            _context.SalesOrders.Add(order);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Result<SalesOrderResponseDto>.Success(MapToResponse(order, parts));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return Result<SalesOrderResponseDto>.Failure($"Failed to create order: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<SalesOrderResponseDto>>> GetOrdersByCustomerAsync(string customerId)
    {
        var orders = await _context.SalesOrders
            .Include(o => o.Items)
                .ThenInclude(i => i.Part)
            .Where(o => o.CustomerId == customerId)
            .AsNoTracking()
            .ToListAsync();

        var response = orders.Select(o => MapToResponse(o, o.Items.Select(i => i.Part).ToList()));
        return Result<IEnumerable<SalesOrderResponseDto>>.Success(response);
    }

    private static SalesOrderResponseDto MapToResponse(SalesOrder order, List<Part> parts) => new()
    {
        Id = order.Id,
        OrderDate = order.OrderDate,
        GrossAmount = order.GrossAmount,
        DiscountAmount = order.DiscountAmount,
        FinalAmount = order.FinalAmount,
        LoyaltyDiscountApplied = order.DiscountAmount > 0,
        Items = order.Items.Select(i => new SalesOrderItemResponseDto
        {
            PartId = i.PartId,
            PartName = parts.FirstOrDefault(p => p.Id == i.PartId)?.Name ?? string.Empty,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
            SubTotal = i.SubTotal
        }).ToList()
    };
}