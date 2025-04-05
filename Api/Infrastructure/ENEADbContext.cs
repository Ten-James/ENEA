using Api.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        foreach (EntityEntry entry in ChangeTracker.Entries())
        {
            if (entry.Entity is not EntityBase entityBase)
            {
                continue;
            }
            switch (entry.State)
            {
                case EntityState.Added:
                    entityBase.CreatedAt = DateTime.UtcNow;
                    entityBase.UpdatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entityBase.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
    }
}