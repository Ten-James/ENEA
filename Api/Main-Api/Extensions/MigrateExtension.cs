using Api.Infrastructure;
using Api.Infrastructure.Fixtures;
using Microsoft.EntityFrameworkCore;

public static class MigrationExtension
{
    public static void MigrateAndSeedDatabase(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ENEADbContext>();
        var logger = services.GetRequiredService<ILogger<ENEADbContext>>();

        try
        {
            logger.LogInformation("Migrating database...");
            context.Database.Migrate();
            InitialDataSeeder.Initialize(services);
            logger.LogInformation("Migration and seeding completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating or seeding the database.");
            throw;
        }
    }
}