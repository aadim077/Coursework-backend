using System.Linq.Expressions;

namespace VehicleParts.Application.Interfaces;

public interface IRepositoryBase<T>
{
    Task<List<T>> FindAllAsync(bool trackChanges = false);
    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false);
    Task<T?> GetByIdAsync(int id);
    void Create(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task SaveChangesAsync();
}