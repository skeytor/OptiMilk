using CattleManagement.API.Data;
using CattleManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CattleManagement.API.Extensions;

internal static class MigrationExtensions
{
    internal static void ApplyMigrations(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
    }
    internal static IServiceCollection AddDatabaseProvider(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSqlServer<AppDbContext>(
            configuration.GetConnectionString("Database"),
            optionsAction: (options) => options.UseSeeding((context, _) =>
            {
                context.Set<Cattle>().AddRange(SampleData.Cattles);
                context.SaveChanges();
            }));
        return services;
    }
}
