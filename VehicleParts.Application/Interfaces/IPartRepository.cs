using VehicleParts.Domain.Entities;

namespace VehicleParts.Application.Interfaces;

public interface IPartRepository : IRepositoryBase<Part>
{
    // IRepositoryBase already covers what we need
}