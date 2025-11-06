using CattleManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CattleManagement.API;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Cattle> Cattles => Set<Cattle>();
}
