using Api.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : EntityBase
{
    protected readonly ENEADbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ENEADbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public Task<int> CountAsync()
    {
        return _dbSet.CountAsync();
    }
    public virtual async Task<IEnumerable<T>> GetAllAsync(int offset, int size)
    {
        var skipped = _dbSet.Skip(offset);
        if (size > 0)
        {
            return await skipped.Take(size).ToListAsync();
        }
        return await skipped.ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        _dbSet.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}