using CattleManagement.API.Data;
using CattleManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CattleManagement.API.Extensions;

internal static class MigrationExtensions
{
    internal static async Task ApplyMigrations(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (await context.Database.CanConnectAsync())
        {
            if (context.Database.GetPendingMigrations().Any())
            {
                app.Logger.LogInformation("Applying database migrations...");
                await context.Database.MigrateAsync();
                app.Logger.LogInformation("Database migrations applied.");
            }
        }
        else
        {
            app.Logger.LogInformation("Creating database...");
            await context.Database.MigrateAsync();
            app.Logger.LogInformation("Database created.");
        }
    }
    internal static IServiceCollection AddDatabaseProvider(
        this IServiceCollection services,
        IConfiguration configuration) => 
        services.AddSqlServer<AppDbContext>(
            configuration.GetConnectionString("Database"),
            sqlOptions => sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null),
            options => options.UseAsyncSeeding(async (context, _, ct) =>
            {
                if (await context.Set<Cattle>().AnyAsync())
                {
                    return;
                }
                await context.Set<Cattle>().AddRangeAsync(SampleData.Cattles);
                await context.SaveChangesAsync(ct);
            }));
}
