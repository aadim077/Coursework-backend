using Microsoft.EntityFrameworkCore;
using VehicleParts.Application.Common;
using VehicleParts.Application.DTOs.Appointments;
using VehicleParts.Application.Interfaces;
using VehicleParts.Domain.Entities;
using VehicleParts.Domain.Enums;
using VehicleParts.Infrastructure.Data;

namespace VehicleParts.Infrastructure.Services;

public class AppointmentService : IAppointmentService
{
    private readonly AppDbContext _db;

    public AppointmentService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Result<AppointmentResponseDto>> BookAppointmentAsync(string customerId, AppointmentRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            return Result<AppointmentResponseDto>.Failure("Invalid customer ID.");
        if (dto == null)
            return Result<AppointmentResponseDto>.Failure("Invalid appointment data.");
        if (dto.AppointmentDateTime <= DateTime.UtcNow)
            return Result<AppointmentResponseDto>.Failure("Appointment date/time must be in the future.");

        var vehicle = await _db.CustomerVehicles
            .FirstOrDefaultAsync(v => v.Id == dto.VehicleId && v.CustomerId == customerId);
        if (vehicle == null)
            return Result<AppointmentResponseDto>.Failure("Vehicle not found or does not belong to customer.");

        var appointment = new Appointment
        {
            CustomerId = customerId,
            VehicleId = dto.VehicleId,
            AppointmentDateTime = dto.AppointmentDateTime,
            Description = dto.Description,
            Status = AppointmentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        await _db.Appointments.AddAsync(appointment);
        await _db.SaveChangesAsync();

        var response = new AppointmentResponseDto
        {
            Id = appointment.Id,
            VehicleId = appointment.VehicleId,
            AppointmentDateTime = appointment.AppointmentDateTime,
            Description = appointment.Description,
            Status = appointment.Status.ToString(),
            CreatedAt = appointment.CreatedAt
        };
        return Result<AppointmentResponseDto>.Success(response);
    }

    public async Task<Result<List<AppointmentResponseDto>>> GetAppointmentsAsync(string customerId)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            return Result<List<AppointmentResponseDto>>.Failure("Invalid customer ID.");

        var appointments = await _db.Appointments
            .AsNoTracking()
            .Where(a => a.CustomerId == customerId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new AppointmentResponseDto
            {
                Id = a.Id,
                VehicleId = a.VehicleId,
                AppointmentDateTime = a.AppointmentDateTime,
                Description = a.Description,
                Status = a.Status.ToString(),
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();
        return Result<List<AppointmentResponseDto>>.Success(appointments);
    }
}

