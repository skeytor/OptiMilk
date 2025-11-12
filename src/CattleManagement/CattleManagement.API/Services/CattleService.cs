using CattleManagement.API.DTOs;
using CattleManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CattleManagement.API.Services;

public class CattleService(AppDbContext context)
{
    public readonly AppDbContext _context = context;
    public ValueTask<Cattle?> GetCattleByIdAsync(Guid cattleId, CancellationToken ct = default) => 
        _context.Cattles.FindAsync([cattleId], ct);

    public Task<List<Cattle>> GetAllAsync() => _context.Cattles.ToListAsync();
    public async Task<Cattle> InsertAsync(CreateCattleRequest request)
    {
        if ( await _context.Cattles.AnyAsync(x => x.TagNumber == request.TagNumber))
        {

        }
        Cattle cattle = new()
        {
            Breed = request.Breed,
            TagNumber = request.TagNumber,
            DateOfBirth = request.DateOfBirth
        };
        await _context.Cattles.AddAsync(cattle);
        await _context.SaveChangesAsync();
        return cattle;
    }
}
