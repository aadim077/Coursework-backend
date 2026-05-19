using VehicleParts.Application.DTOs.Customers;
using VehicleParts.Application.Common;

namespace VehicleParts.Application.Interfaces;

public interface ICustomerProfileService
{
    Task<Result<CustomerProfileDto>> GetProfileAsync(string customerId);
    Task<Result> UpdateProfileAsync(string customerId, string fullName, string phoneNumber);

    Task<Result<List<CustomerVehicleResponseDto>>> GetVehiclesAsync(string customerId);
    Task<Result<CustomerVehicleResponseDto>> GetVehicleByIdAsync(string customerId, int vehicleId);
    Task<Result> CreateVehicleAsync(string customerId, CustomerVehicleRequestDto dto);
    Task<Result> UpdateVehicleAsync(string customerId, int vehicleId, CustomerVehicleRequestDto dto);
    Task<Result> DeleteVehicleAsync(string customerId, int vehicleId);
}

