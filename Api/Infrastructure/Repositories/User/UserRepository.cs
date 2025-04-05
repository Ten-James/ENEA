using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repositories.User;

public class UserRepository : Repository<Models.User>, IUserRepository
{

    /// <inheritdoc />
    public UserRepository(ENEADbContext context) : base(context)
    {
    }
    public async Task<Models.User?> GetByEmailAsync(string email)
    {
        var user = await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        return user;
    }
    public async Task<Models.User?> GetByEmailAndPasswordAsync(string email, string password)
    {
        var user = await _dbSet.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        return user;
    }
}