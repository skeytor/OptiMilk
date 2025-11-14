using CattleManagement.API.Data;
using CattleManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CattleManagement.API.Extensions;

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
            configuration.GetConnectionString("CattleDatabase"),
            optionsAction: (options) => options.UseSeeding((context, _) =>
            {
                context.Set<Cattle>().AddRange(SampleData.Cattles);
                context.SaveChanges();
            }));
        return services;
    }
}
