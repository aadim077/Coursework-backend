using VehicleParts.Application.DTOs.Reviews;
using VehicleParts.Application.Common;

namespace VehicleParts.Application.Interfaces;

public interface IReviewService
{
    Task<Result<ReviewResponseDto>> SubmitReviewAsync(string customerId, ReviewRequestDto dto);
    Task<Result<List<ReviewResponseDto>>> GetReviewsAsync(string customerId);
}

