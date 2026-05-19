using VehicleParts.Application.Interfaces;
using VehicleParts.Domain.Entities;
using VehicleParts.Infrastructure.Data;

namespace VehicleParts.Infrastructure.Repositories;

public class PartRepository : RepositoryBase<Part>, IPartRepository
{
    public PartRepository(AppDbContext context) : base(context) { }
}