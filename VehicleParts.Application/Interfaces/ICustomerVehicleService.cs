using VehicleParts.Application.DTOs;

namespace VehicleParts.Application.Interfaces;

public interface ICustomerVehicleService
{
    Task<(bool Success, string Message, string? CustomerId)> RegisterCustomerWithVehicleAsync(RegisterCustomerWithVehicleDto dto);
    Task<CustomerDetailsDto?> GetCustomerByIdAsync(string customerId);
    Task<CustomerVehicleDetailDto?> GetCustomerVehicleDetailAsync(string customerId, int vehicleId);
    Task<List<CustomerDetailsDto>> SearchCustomersByVehicleNumberAsync(string vehicleNumber);
    Task<List<CustomerDetailsDto>> SearchCustomersByPhoneAsync(string phoneNumber);
    Task<List<CustomerDetailsDto>> SearchCustomersByIdAsync(string customerId);
    Task<List<CustomerDetailsDto>> SearchCustomersByNameAsync(string name);
}
