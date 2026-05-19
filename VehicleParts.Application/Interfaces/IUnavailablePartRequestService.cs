using VehicleParts.Application.DTOs.UnavailableParts;
using VehicleParts.Application.Common;

namespace VehicleParts.Application.Interfaces;

public interface IUnavailablePartRequestService
{
    Task<Result<UnavailablePartRequestResponseDto>> RequestUnavailablePartAsync(string customerId, UnavailablePartRequestDto dto);
    Task<Result<List<UnavailablePartRequestResponseDto>>> GetUnavailablePartRequestsAsync(string customerId);
}

