namespace Api.Infrastructure.Repositories.User;

using User=Models.User;

public interface IUserRepository : IRepository<User>
{
    public Task<User?> GetByEmailAsync(string email);

    public Task<User?> GetByEmailAndPasswordAsync(string email, string password);
}