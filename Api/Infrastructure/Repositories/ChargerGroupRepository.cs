using Api.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class ChargerGroupRepository : Repository<ChargerGroup>
{
    public ChargerGroupRepository(ENEADbContext context) : base(context)
    {
    }
    public override async Task<IEnumerable<ChargerGroup>> GetAllAsync(int offset, int size)
    {
        var skipped = _dbSet
            .AsNoTracking()
            .Include(x => x.Chargers)
            .Skip(offset);

        if (size > 0)
        {
            return await skipped.Take(size).ToListAsync();
        }

        return await skipped.ToListAsync();
    }

    public override async Task<ChargerGroup?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Chargers)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}