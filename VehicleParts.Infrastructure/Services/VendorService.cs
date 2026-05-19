using Microsoft.EntityFrameworkCore;
using VehicleParts.Application.Common;
using VehicleParts.Application.DTOs.Vendor;
using VehicleParts.Application.Interfaces;
using VehicleParts.Infrastructure.Data;

namespace VehicleParts.Infrastructure.Services;

internal sealed class VendorService : IVendorService
{
    private readonly AppDbContext _context;

    public VendorService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<VendorDto>>> GetAllVendorsAsync(CancellationToken cancellationToken = default)
    {
        var vendors = await _context.Vendors
            .AsNoTracking()
            .OrderByDescending(v => v.CreatedAt)
            .Select(v => MapToDto(v))
            .ToListAsync(cancellationToken);

        return Result<List<VendorDto>>.Success(vendors);
    }

    public async Task<Result<VendorDto>> GetVendorByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var vendor = await _context.Vendors
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        if (vendor == null)
            return Result<VendorDto>.Failure("Vendor not found.");

        return Result<VendorDto>.Success(MapToDto(vendor));
    }

    public async Task<Result<VendorDto>> CreateVendorAsync(CreateVendorRequest request, CancellationToken cancellationToken = default)
    {
        var emailExists = await _context.Vendors
            .AnyAsync(v => v.Email.ToLower() == request.Email.ToLower(), cancellationToken);

        if (emailExists)
            return Result<VendorDto>.Failure("A vendor with this email already exists.");

        var vendor = new Domain.Entities.Vendor
        {
            Name = request.Name,
            ContactPerson = request.ContactPerson,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            CreatedAt = DateTime.UtcNow
        };

        _context.Vendors.Add(vendor);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<VendorDto>.Success(MapToDto(vendor), "Vendor created successfully.");
    }

    public async Task<Result<VendorDto>> UpdateVendorAsync(int id, UpdateVendorRequest request, CancellationToken cancellationToken = default)
    {
        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        if (vendor == null)
            return Result<VendorDto>.Failure("Vendor not found.");

        if (!string.IsNullOrWhiteSpace(request.Email) &&
            !request.Email.Equals(vendor.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailExists = await _context.Vendors
                .AnyAsync(v => v.Id != id && v.Email.ToLower() == request.Email.ToLower(), cancellationToken);

            if (emailExists)
                return Result<VendorDto>.Failure("A vendor with this email already exists.");
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
            vendor.Name = request.Name;

        if (!string.IsNullOrWhiteSpace(request.ContactPerson))
            vendor.ContactPerson = request.ContactPerson;

        if (!string.IsNullOrWhiteSpace(request.Email))
            vendor.Email = request.Email;

        if (!string.IsNullOrWhiteSpace(request.Phone))
            vendor.Phone = request.Phone;

        if (!string.IsNullOrWhiteSpace(request.Address))
            vendor.Address = request.Address;

        vendor.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<VendorDto>.Success(MapToDto(vendor), "Vendor updated successfully.");
    }

    public async Task<Result> DeleteVendorAsync(int id, CancellationToken cancellationToken = default)
    {
        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        if (vendor == null)
            return Result.Failure("Vendor not found.");

        var hasLinkedParts = await _context.Parts
            .AnyAsync(p => p.VendorId == id, cancellationToken);

        if (hasLinkedParts)
            return Result.Failure("Cannot delete vendor with linked parts. Remove or reassign parts first.");

        var hasLinkedInvoices = await _context.PurchaseInvoices
            .AnyAsync(pi => pi.VendorId == id, cancellationToken);

        if (hasLinkedInvoices)
            return Result.Failure("Cannot delete vendor with linked purchase invoices.");

        _context.Vendors.Remove(vendor);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success("Vendor deleted successfully.");
    }

    private static VendorDto MapToDto(Domain.Entities.Vendor vendor)
    {
        return new VendorDto
        {
            Id = vendor.Id,
            Name = vendor.Name,
            ContactPerson = vendor.ContactPerson,
            Email = vendor.Email,
            Phone = vendor.Phone,
            Address = vendor.Address,
            CreatedAt = vendor.CreatedAt,
            UpdatedAt = vendor.UpdatedAt
        };
    }
}
