using VehicleParts.Application.Common;
using VehicleParts.Application.DTOs.Staff;

namespace VehicleParts.Application.Interfaces;

public interface IStaffService
{
    Task<Result<List<StaffDto>>> GetAllStaffAsync(CancellationToken cancellationToken = default);
    Task<Result<StaffDto>> GetStaffByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<StaffDto>> CreateStaffAsync(CreateStaffRequest request, CancellationToken cancellationToken = default);
    Task<Result<StaffDto>> UpdateStaffAsync(string id, UpdateStaffRequest request, CancellationToken cancellationToken = default);
    Task<Result> ResetStaffPasswordAsync(string id, UpdateStaffPasswordRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteStaffAsync(string id, CancellationToken cancellationToken = default);
}
