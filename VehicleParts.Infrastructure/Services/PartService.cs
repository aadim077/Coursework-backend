using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using VehicleParts.Application.Common;
using VehicleParts.Application.DTOs.Parts;
using VehicleParts.Application.Interfaces;
using VehicleParts.Domain.Entities;
using VehicleParts.Infrastructure.Data;

namespace VehicleParts.Infrastructure.Services;

public class PartService : IPartService
{
    private readonly AppDbContext _context;

    public PartService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IEnumerable<PartResponseDto>>> GetAllAsync()
    {
        var parts = await _context.Parts
            .Include(p => p.Vendor)
            .AsNoTracking()
            .ToListAsync();

        var response = parts.Select(MapToResponse);
        return Result<IEnumerable<PartResponseDto>>.Success(response);
    }

    public async Task<Result<PartResponseDto>> GetByIdAsync(int id)
    {
        var part = await _context.Parts
            .Include(p => p.Vendor)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (part is null)
            return Result<PartResponseDto>.Failure($"Part with ID {id} not found.");

        return Result<PartResponseDto>.Success(MapToResponse(part));
    }

    public async Task<Result<PartResponseDto>> CreateAsync(CreatePartDto dto)
    {
        var vendorExists = await _context.Vendors.AnyAsync(v => v.Id == dto.VendorId);
        if (!vendorExists)
            return Result<PartResponseDto>.Failure($"Vendor with ID {dto.VendorId} not found.");

        var part = new Part
        {
            Name = dto.Name,
            Description = dto.Description,
            Category = dto.Category,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            MinimumStockLevel = dto.MinimumStockLevel,
            VendorId = dto.VendorId
        };

        _context.Parts.Add(part);
        await _context.SaveChangesAsync();

        await _context.Entry(part).Reference(p => p.Vendor).LoadAsync();

        return Result<PartResponseDto>.Success(MapToResponse(part));
    }

    public async Task<Result<PartResponseDto>> UpdateAsync(int id, UpdatePartDto dto)
    {
        var part = await _context.Parts
            .Include(p => p.Vendor)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (part is null)
            return Result<PartResponseDto>.Failure($"Part with ID {id} not found.");

        var vendorExists = await _context.Vendors.AnyAsync(v => v.Id == dto.VendorId);
        if (!vendorExists)
            return Result<PartResponseDto>.Failure($"Vendor with ID {dto.VendorId} not found.");

        part.Name = dto.Name;
        part.Description = dto.Description;
        part.Category = dto.Category;
        part.Price = dto.Price;
        part.MinimumStockLevel = dto.MinimumStockLevel;
        part.VendorId = dto.VendorId;

        await _context.SaveChangesAsync();
        await _context.Entry(part).Reference(p => p.Vendor).LoadAsync();

        return Result<PartResponseDto>.Success(MapToResponse(part));
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        var part = await _context.Parts.FindAsync(id);

        if (part is null)
            return Result<bool>.Failure($"Part with ID {id} not found.");

        _context.Parts.Remove(part);
        await _context.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    private static PartResponseDto MapToResponse(Part part) => new()
    {
        Id = part.Id,
        Name = part.Name,
        Description = part.Description,
        Category = part.Category,
        Price = part.Price,
        StockQuantity = part.StockQuantity,
        MinimumStockLevel = part.MinimumStockLevel,
        VendorId = part.VendorId,
        VendorName = part.Vendor?.Name ?? string.Empty
    };
}