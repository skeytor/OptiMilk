using Microsoft.EntityFrameworkCore;
using MilkingYield.API.Clients;
using MilkingYield.API.Data;
using MilkingYield.API.Models;

namespace MilkingYield.API.Extentions;

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
        services.AddDbContext<AppDbContext>(
            options => options.UseNpgsql(configuration.GetConnectionString("Database"))
            .UseSeeding((context, _) =>
            {
                context.Set<MilkingRecord>().AddRange(SampleData.MilkingYields);
                context.SaveChanges();
            }));
        return services;
    }
}
