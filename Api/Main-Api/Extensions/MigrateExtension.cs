using Api.Infrastructure;

public static class MigrationExtension
{
    public static void MigrateAndSeedDatabase(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        IServiceProvider services = scope.ServiceProvider;
        ENEADbContext context = services.GetRequiredService<ENEADbContext>();
        ILogger<ENEADbContext> logger = services.GetRequiredService<ILogger<ENEADbContext>>();

        try
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            //InitialDataSeeder.Initialize(services);
            logger.LogInformation("Migration and seeding completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating or seeding the database.");
            throw;
        }
    }
}