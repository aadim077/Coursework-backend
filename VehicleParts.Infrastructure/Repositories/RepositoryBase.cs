using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using VehicleParts.Application.Interfaces;
using VehicleParts.Domain.Entities;
using VehicleParts.Infrastructure.Data;

namespace VehicleParts.Infrastructure.Repositories;

public class RepositoryBase<T> : IRepositoryBase<T> where T : BaseEntity
{
    private readonly AppDbContext _context;

    public RepositoryBase(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<T>> FindAllAsync(bool trackChanges = false)
        => trackChanges
            ? await _context.Set<T>().ToListAsync()
            : await _context.Set<T>().AsNoTracking().ToListAsync();

    public IQueryable<T> FindByCondition(
        Expression<Func<T, bool>> expression, bool trackChanges = false)
        => trackChanges
            ? _context.Set<T>().Where(expression)
            : _context.Set<T>().AsNoTracking().Where(expression);

    public async Task<T?> GetByIdAsync(int id)
        => await _context.Set<T>().FindAsync(id);

    public void Create(T entity) => _context.Set<T>().Add(entity);
    public void Update(T entity) => _context.Set<T>().Update(entity);
    public void Delete(T entity) => _context.Set<T>().Remove(entity);

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}