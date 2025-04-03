using Api.Infrastructure;
using Api.Infrastructure.Repositories;
using BussinesLogic.Services;
using Main_Api.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Main_Api;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddDbContext<ENEADbContext>(opt => opt.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=enea;Username=enea;Password=enea")
        );

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped(typeof(IService<,,,,>), typeof(Service<,,,,>));

        builder.Services.AddAutoMapper(typeof(ENEADbContext).Assembly);

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MigrateAndSeedDatabase();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapGet("/api", handler: () => Results.Json(ApiSpecificationHelper.GetApiSpecification()));
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}