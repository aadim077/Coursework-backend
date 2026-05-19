using VehicleParts.Application.Common;
using VehicleParts.Application.DTOs.Vendor;

namespace VehicleParts.Application.Interfaces;

public interface IVendorService
{
    Task<Result<List<VendorDto>>> GetAllVendorsAsync(CancellationToken cancellationToken = default);
    Task<Result<VendorDto>> GetVendorByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<VendorDto>> CreateVendorAsync(CreateVendorRequest request, CancellationToken cancellationToken = default);
    Task<Result<VendorDto>> UpdateVendorAsync(int id, UpdateVendorRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteVendorAsync(int id, CancellationToken cancellationToken = default);
}
