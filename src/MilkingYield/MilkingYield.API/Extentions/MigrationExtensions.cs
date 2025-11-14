using Microsoft.EntityFrameworkCore;
using MilkingYield.API.Clients;
using MilkingYield.API.Data;
using MilkingYield.API.Models;

namespace MilkingYield.API.Extentions;

internal static class MigrationExtensions
{
    internal static WebApplication ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            AppDbContext context = services.GetRequiredService<AppDbContext>();
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
        }
        catch (Exception ex)
        {
            ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating or seeding the database.");
        }
        return app;
    }
    internal static IServiceCollection AddDatabaseProvider(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSqlServer<AppDbContext>(
            configuration.GetConnectionString("MilkingYieldDatabase"),
            optionsAction: (options) => options.UseSeeding((context, _) =>
            {
                context.Set<MilkingRecord>().AddRange(SampleData.MilkingYields);
                context.SaveChanges();
            }));
        return services;
    }
}
