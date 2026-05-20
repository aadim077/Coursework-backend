using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VehicleParts.Application.DTOs;
using VehicleParts.Application.Interfaces;
using VehicleParts.Domain.Entities;
using VehicleParts.Infrastructure.Data;

namespace VehicleParts.Infrastructure.Services;

public class CustomerVehicleService : ICustomerVehicleService
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public CustomerVehicleService(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<(bool Success, string Message, string? CustomerId)> RegisterCustomerWithVehicleAsync(RegisterCustomerWithVehicleDto dto)
    {
        try
        {
            var user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                FullName = $"{dto.FirstName} {dto.LastName}",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, $"Failed to create customer: {errors}", null);
            }

            await _userManager.AddToRoleAsync(user, "Customer");

            var vehicle = new Vehicle
            {
                VehicleNumber = dto.VehicleNumber,
                Make = dto.Make,
                Model = dto.Model,
                Year = dto.Year,
                Color = dto.Color,
                VIN = dto.VIN,
                CustomerId = user.Id,
                RegisteredDate = DateTime.UtcNow
            };

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            return (true, "Customer registered successfully with vehicle details", user.Id);
        }
        catch (Exception ex)
        {
            return (false, $"Error registering customer: {ex.Message}", null);
        }
    }

    public async Task<CustomerDetailsDto?> GetCustomerByIdAsync(string customerId)
    {
        var user = await _userManager.FindByIdAsync(customerId);
        if (user == null)
            return null;

        var vehicles = await _context.Vehicles
            .Where(v => v.CustomerId == customerId)
            .Select(v => new VehicleDto
            {
                Id = v.Id,
                VehicleNumber = v.VehicleNumber,
                Make = v.Make,
                Model = v.Model,
                Year = v.Year,
                Color = v.Color,
                VIN = v.VIN,
                RegisteredDate = v.RegisteredDate,
                LastServiceDate = v.LastServiceDate
            })
            .ToListAsync();

        return new CustomerDetailsDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            RegisteredDate = user.CreatedAt,
            Vehicles = vehicles
        };
    }

    public async Task<CustomerVehicleDetailDto?> GetCustomerVehicleDetailAsync(string customerId, int vehicleId)
    {
        var user = await _userManager.FindByIdAsync(customerId);
        if (user == null)
            return null;

        var vehicle = await _context.Vehicles
            .Include(v => v.ServiceHistories)
            .FirstOrDefaultAsync(v => v.Id == vehicleId && v.CustomerId == customerId);

        if (vehicle == null)
            return null;

        return new CustomerVehicleDetailDto
        {
            CustomerId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Vehicle = new VehicleDto
            {
                Id = vehicle.Id,
                VehicleNumber = vehicle.VehicleNumber,
                Make = vehicle.Make,
                Model = vehicle.Model,
                Year = vehicle.Year,
                Color = vehicle.Color,
                VIN = vehicle.VIN,
                RegisteredDate = vehicle.RegisteredDate,
                LastServiceDate = vehicle.LastServiceDate
            },
            ServiceHistory = vehicle.ServiceHistories
                .Select(sh => new ServiceHistoryDto
                {
                    Id = sh.Id,
                    Description = sh.Description,
                    ServiceDate = sh.ServiceDate,
                    Cost = sh.Cost,
                    Notes = sh.Notes
                })
                .ToList()
        };
    }

    public async Task<List<CustomerDetailsDto>> SearchCustomersByVehicleNumberAsync(string vehicleNumber)
    {
        var vehicles = await _context.Vehicles
            .Where(v => v.VehicleNumber.Contains(vehicleNumber))
            .Select(v => v.CustomerId)
            .Distinct()
            .ToListAsync();

        var customers = new List<CustomerDetailsDto>();
        foreach (var customerId in vehicles)
        {
            var customer = await GetCustomerByIdAsync(customerId);
            if (customer != null)
                customers.Add(customer);
        }

        return customers;
    }

    public async Task<List<CustomerDetailsDto>> SearchCustomersByPhoneAsync(string phoneNumber)
    {
        var users = await _userManager.Users
            .Where(u => u.PhoneNumber != null && u.PhoneNumber.Contains(phoneNumber))
            .ToListAsync();

        var customers = new List<CustomerDetailsDto>();
        foreach (var user in users)
        {
            var isCustomer = await _userManager.IsInRoleAsync(user, "Customer");
            if (isCustomer)
            {
                var customer = await GetCustomerByIdAsync(user.Id);
                if (customer != null)
                    customers.Add(customer);
            }
        }

        return customers;
    }

    public async Task<List<CustomerDetailsDto>> SearchCustomersByIdAsync(string customerId)
    {
        var customer = await GetCustomerByIdAsync(customerId);
        return customer != null ? new List<CustomerDetailsDto> { customer } : new List<CustomerDetailsDto>();
    }

    public async Task<List<CustomerDetailsDto>> SearchCustomersByNameAsync(string name)
    {
        var users = await _userManager.Users
            .Where(u => u.FullName != null && u.FullName.Contains(name))
            .ToListAsync();

        var customers = new List<CustomerDetailsDto>();
        foreach (var user in users)
        {
            var isCustomer = await _userManager.IsInRoleAsync(user, "Customer");
            if (isCustomer)
            {
                var customer = await GetCustomerByIdAsync(user.Id);
                if (customer != null)
                    customers.Add(customer);
            }
        }

        return customers;
    }
}
