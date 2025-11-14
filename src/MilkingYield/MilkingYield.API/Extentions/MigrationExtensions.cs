using Microsoft.EntityFrameworkCore;
using MilkingYield.API.Data;
using MilkingYield.API.Models;

namespace MilkingYield.API.Extentions;

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
            }
        }
        else
        {
            app.Logger.LogInformation("Creating database...");
            await context.Database.MigrateAsync();
        }
    }
    internal static IServiceCollection AddDatabaseProvider(
        this IServiceCollection services,
        IConfiguration configuration) => 
        services.AddNpgsql<AppDbContext>(
            configuration.GetConnectionString("Database"),
            npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorCodesToAdd: null),
            options => options.UseAsyncSeeding(async (context, _, ct) =>
            {
                if (await context.Set<MilkingRecord>().AnyAsync())
                {
                    return;
                }
                await context.Set<MilkingRecord>().AddRangeAsync(SampleData.MilkingYields);
                await context.SaveChangesAsync(ct);
            }));
}
