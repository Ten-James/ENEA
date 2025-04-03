using Api.Infrastructure.Models;

namespace Api.Infrastructure.Repositories;

public interface IRepository<T> where T : EntityBase
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}