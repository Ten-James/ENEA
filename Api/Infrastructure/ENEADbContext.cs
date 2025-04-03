using Api.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure;

public class ENEADbContext : DbContext
{
    public ENEADbContext(DbContextOptions<ENEADbContext> options) : base(options)
    {
    }

    public DbSet<ChargerGroup> ChargerGroups { get; set; }

    public DbSet<Charger> Chargers { get; set; }
    public DbSet<ChargerEvent> ChargerEvents { get; set; }

    public DbSet<User> Users { get; set; }
}