using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VehicleParts.Application.Common;
using VehicleParts.Application.DTOs.CustomerHistory;
using VehicleParts.Application.Interfaces;
using VehicleParts.Domain.Entities;
using VehicleParts.Infrastructure.Data;

namespace VehicleParts.Infrastructure.Services
{
    public class CustomerHistoryService : ICustomerHistoryService
    {
        private readonly AppDbContext _db;

        public CustomerHistoryService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Result<List<ServiceHistoryDto>>> GetServiceHistoryAsync(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
                return Result<List<ServiceHistoryDto>>.Failure("Customer id is required.");

            var history = await _db.Set<Appointment>()
                .AsNoTracking()
                .Where(appointment => appointment.CustomerId == customerId)
                .OrderByDescending(appointment => appointment.AppointmentDateTime)
                .Select(appointment => new ServiceHistoryDto
                {
                    AppointmentId = appointment.Id,
                    VehicleId = appointment.VehicleId,
                    VehicleMake = appointment.Vehicle.Make,
                    VehicleModel = appointment.Vehicle.Model,
                    VehicleNumber = appointment.Vehicle.VehicleNumber,
                    AppointmentDateTime = appointment.AppointmentDateTime,
                    Description = appointment.Description,
                    Status = appointment.Status.ToString()
                })
                .ToListAsync();

            return Result<List<ServiceHistoryDto>>.Success(history);
        }

        public async Task<Result<List<PurchaseHistoryDto>>> GetPurchaseHistoryAsync(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
                return Result<List<PurchaseHistoryDto>>.Failure("Customer id is required.");

            var history = await _db.Set<CustomerPurchase>()
                .AsNoTracking()
                .Where(purchase => purchase.CustomerId == customerId)
                .OrderByDescending(purchase => purchase.PurchaseDate)
                .Select(purchase => new PurchaseHistoryDto
                {
                    Id = purchase.Id,
                    PurchaseDate = purchase.PurchaseDate,
                    ReferenceNumber = purchase.ReferenceNumber,
                    TotalAmount = purchase.TotalAmount,
                    Status = purchase.Status,
                    Items = purchase.Items
                        .OrderBy(item => item.Id)
                        .Select(item => new PurchaseHistoryItemDto
                        {
                            Id = item.Id,
                            PartId = item.PartId,
                            ItemName = item.ItemName,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice,
                            LineTotal = item.LineTotal
                        })
                        .ToList()
                })
                .ToListAsync();

            return Result<List<PurchaseHistoryDto>>.Success(history);
        }
    }
}