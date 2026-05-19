using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleParts.Application.Common;
using VehicleParts.Application.DTOs.CustomerHistory;

namespace VehicleParts.Application.Interfaces
{
    public interface ICustomerHistoryService
    {
        Task<Result<List<ServiceHistoryDto>>> GetServiceHistoryAsync(string customerId);
        Task<Result<List<PurchaseHistoryDto>>> GetPurchaseHistoryAsync(string customerId);
    }
}

