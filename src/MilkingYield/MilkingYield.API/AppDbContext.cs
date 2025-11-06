using Microsoft.EntityFrameworkCore;
using MilkingYield.API.Models;

namespace MilkingYield.API;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<MilkingRecord> MilkingYields => Set<MilkingRecord>();
}
