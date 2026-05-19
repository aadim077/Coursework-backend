using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VehicleParts.Application.Common;
using VehicleParts.Application.DTOs.Customers;
using VehicleParts.Application.Interfaces;
using VehicleParts.Domain.Entities;
using VehicleParts.Infrastructure.Data;

namespace VehicleParts.Infrastructure.Services;

public class CustomerProfileService : ICustomerProfileService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDbContext _db;

    public CustomerProfileService(UserManager<AppUser> userManager, AppDbContext db)
    {
        _userManager = userManager;
        _db = db;
    }

    public async Task<Result<CustomerProfileDto>> GetProfileAsync(string customerId)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == customerId);
        if (user == null)
            return Result<CustomerProfileDto>.Failure("Customer not found.");
        var dto = new CustomerProfileDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email
        };
        return Result<CustomerProfileDto>.Success(dto);
    }

    public async Task<Result> UpdateProfileAsync(string customerId, string fullName, string phoneNumber)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == customerId);
        if (user == null)
            return Result.Failure("Customer not found.");
        user.FullName = fullName;
        user.PhoneNumber = phoneNumber;
        await _userManager.UpdateAsync(user);
        return Result.Success();
    }

    public async Task<Result<List<CustomerVehicleResponseDto>>> GetVehiclesAsync(string customerId)
    {
        var vehicles = await _db.CustomerVehicles
            .Where(v => v.CustomerId == customerId)
            .Select(v => new CustomerVehicleResponseDto
            {
                Id = v.Id,
                VehicleNumber = v.VehicleNumber,
                Make = v.Make,
                Model = v.Model,
                Year = v.Year,
                Color = v.Color
            })
            .ToListAsync();
        return Result<List<CustomerVehicleResponseDto>>.Success(vehicles);
    }

    public async Task<Result<CustomerVehicleResponseDto>> GetVehicleByIdAsync(string customerId, int vehicleId)
    {
        var vehicle = await _db.CustomerVehicles
            .Where(v => v.CustomerId == customerId && v.Id == vehicleId)
            .Select(v => new CustomerVehicleResponseDto
            {
                Id = v.Id,
                VehicleNumber = v.VehicleNumber,
                Make = v.Make,
                Model = v.Model,
                Year = v.Year,
                Color = v.Color
            })
            .FirstOrDefaultAsync();
        if (vehicle == null)
            return Result<CustomerVehicleResponseDto>.Failure("Vehicle not found.");
        return Result<CustomerVehicleResponseDto>.Success(vehicle);
    }

    public async Task<Result> CreateVehicleAsync(string customerId, CustomerVehicleRequestDto dto)
    {
        var exists = await _db.CustomerVehicles.AnyAsync(v => v.VehicleNumber == dto.VehicleNumber);
        if (exists)
            return Result.Failure("A vehicle with this number already exists.");
        var vehicle = new CustomerVehicle
        {
            CustomerId = customerId,
            VehicleNumber = dto.VehicleNumber,
            Make = dto.Make,
            Model = dto.Model,
            Year = dto.Year,
            Color = dto.Color
        };
        _db.CustomerVehicles.Add(vehicle);
        await _db.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> UpdateVehicleAsync(string customerId, int vehicleId, CustomerVehicleRequestDto dto)
    {
        var vehicle = await _db.CustomerVehicles.FirstOrDefaultAsync(v => v.Id == vehicleId && v.CustomerId == customerId);
        if (vehicle == null)
            return Result.Failure("Vehicle not found.");
        var duplicate = await _db.CustomerVehicles.AnyAsync(v => v.VehicleNumber == dto.VehicleNumber && v.Id != vehicleId);
        if (duplicate)
            return Result.Failure("A vehicle with this number already exists.");
        vehicle.VehicleNumber = dto.VehicleNumber;
        vehicle.Make = dto.Make;
        vehicle.Model = dto.Model;
        vehicle.Year = dto.Year;
        vehicle.Color = dto.Color;
        await _db.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> DeleteVehicleAsync(string customerId, int vehicleId)
    {
        var vehicle = await _db.CustomerVehicles.FirstOrDefaultAsync(v => v.Id == vehicleId && v.CustomerId == customerId);
        if (vehicle == null)
            return Result.Failure("Vehicle not found.");
        _db.CustomerVehicles.Remove(vehicle);
        await _db.SaveChangesAsync();
        return Result.Success();
    }
}
