using Api.Infrastructure.Models;

namespace Api.Infrastructure.Repositories;

public interface IRepository<T> where T : EntityBase
{
    Task<int> CountAsync();
    Task<IEnumerable<T>> GetAllAsync(int offset, int size);
    Task<T?> GetByIdAsync(Guid id);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}