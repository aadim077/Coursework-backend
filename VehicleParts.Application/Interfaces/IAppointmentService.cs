using VehicleParts.Application.DTOs.Appointments;
using VehicleParts.Application.Common;

namespace VehicleParts.Application.Interfaces;

public interface IAppointmentService
{
    Task<Result<AppointmentResponseDto>> BookAppointmentAsync(string customerId, AppointmentRequestDto dto);
    Task<Result<List<AppointmentResponseDto>>> GetAppointmentsAsync(string customerId);
}

