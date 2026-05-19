using Microsoft.EntityFrameworkCore;
using VehicleParts.Application.Common;
using VehicleParts.Application.DTOs.Reviews;
using VehicleParts.Application.Interfaces;
using VehicleParts.Domain.Entities;
using VehicleParts.Domain.Enums;
using VehicleParts.Infrastructure.Data;

namespace VehicleParts.Infrastructure.Services;

public class ReviewService : IReviewService
{
    private readonly AppDbContext _db;

    public ReviewService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<ReviewResponseDto>> SubmitReviewAsync(string customerId, ReviewRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            return Result<ReviewResponseDto>.Failure("Invalid customer ID.");
        if (dto == null)
            return Result<ReviewResponseDto>.Failure("Invalid review data.");
        if (dto.Rating < 1 || dto.Rating > 5)
            return Result<ReviewResponseDto>.Failure("Rating must be between 1 and 5.");
        if (string.IsNullOrWhiteSpace(dto.Comment))
            return Result<ReviewResponseDto>.Failure("Comment is required.");

        var review = new Review
        {
            CustomerId = customerId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            CreatedAt = DateTime.UtcNow
        };
        await _db.Reviews.AddAsync(review);
        await _db.SaveChangesAsync();

        var response = new ReviewResponseDto
        {
            Id = review.Id,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        };
        return Result<ReviewResponseDto>.Success(response);
    }

    public async Task<Result<List<ReviewResponseDto>>> GetReviewsAsync(string customerId)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            return Result<List<ReviewResponseDto>>.Failure("Invalid customer ID.");

        var reviews = await _db.Reviews
            .AsNoTracking()
            .Where(r => r.CustomerId == customerId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewResponseDto
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
        return Result<List<ReviewResponseDto>>.Success(reviews);
    }
}

