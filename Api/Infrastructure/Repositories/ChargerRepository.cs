using Api.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories;

public class ChargerRepository : Repository<Charger>
{

    public ChargerRepository(ENEADbContext context) : base(context)
    {
    }

    public override async Task<Charger?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(c => c.Events)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}