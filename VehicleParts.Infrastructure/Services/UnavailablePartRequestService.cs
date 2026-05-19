using Microsoft.EntityFrameworkCore;
using VehicleParts.Application.Common;
using VehicleParts.Application.DTOs.UnavailableParts;
using VehicleParts.Application.Interfaces;
using VehicleParts.Domain.Entities;
using VehicleParts.Domain.Enums;
using VehicleParts.Infrastructure.Data;

namespace VehicleParts.Infrastructure.Services;

public class UnavailablePartRequestService : IUnavailablePartRequestService
{
    private readonly AppDbContext _db;

    public UnavailablePartRequestService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<UnavailablePartRequestResponseDto>> RequestUnavailablePartAsync(string customerId, UnavailablePartRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            return Result<UnavailablePartRequestResponseDto>.Failure("Invalid customer ID.");
        if (dto == null)
            return Result<UnavailablePartRequestResponseDto>.Failure("Invalid request data.");
        if (string.IsNullOrWhiteSpace(dto.PartName))
            return Result<UnavailablePartRequestResponseDto>.Failure("Part name is required.");
        if (string.IsNullOrWhiteSpace(dto.Description))
            return Result<UnavailablePartRequestResponseDto>.Failure("Description is required.");

        var request = new UnavailablePartRequest
        {
            CustomerId = customerId,
            PartName = dto.PartName,
            Description = dto.Description,
            Status = UnavailablePartRequestStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        await _db.UnavailablePartRequests.AddAsync(request);
        await _db.SaveChangesAsync();

        var response = new UnavailablePartRequestResponseDto
        {
            Id = request.Id,
            PartName = request.PartName,
            Description = request.Description,
            Status = request.Status.ToString(),
            CreatedAt = request.CreatedAt
        };
        return Result<UnavailablePartRequestResponseDto>.Success(response);
    }

    public async Task<Result<List<UnavailablePartRequestResponseDto>>> GetUnavailablePartRequestsAsync(string customerId)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            return Result<List<UnavailablePartRequestResponseDto>>.Failure("Invalid customer ID.");

        var requests = await _db.UnavailablePartRequests
            .AsNoTracking()
            .Where(r => r.CustomerId == customerId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new UnavailablePartRequestResponseDto
            {
                Id = r.Id,
                PartName = r.PartName,
                Description = r.Description,
                Status = r.Status.ToString(),
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
        return Result<List<UnavailablePartRequestResponseDto>>.Success(requests);
    }
}

