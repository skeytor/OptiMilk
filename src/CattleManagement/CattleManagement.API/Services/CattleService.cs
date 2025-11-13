using CattleManagement.API.DTOs;
using CattleManagement.API.Events;
using CattleManagement.API.Models;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace CattleManagement.API.Services;

public sealed class CattleService(AppDbContext context, CattleKafkaProducerService producer)
{
    private readonly AppDbContext _context = context;
    private readonly CattleKafkaProducerService _producer = producer;

    public async Task<Result<Cattle>> GetCattleByIdAsync(Guid id, CancellationToken ct = default) 
    {
        if (await _context.Cattles.FindAsync([id], ct) is not Cattle cattle)
        {
            return Result.Failure<Cattle>(Error.NotFound("Cattle.NotFound", $"Cattle with id {id} was not found."));
        }
        return Result.Success(cattle);
    }
    public Task<List<Cattle>> GetAllAsync() => _context.Cattles.ToListAsync();
    public async Task<Result<Cattle>> InsertAsync(CreateCattleRequest request)
    {
        if ( await _context.Cattles.AnyAsync(x => x.TagNumber == request.TagNumber))
        {
            return Result.Failure<Cattle>(Error.Failure(
                "Cattle.TagNumberExists", 
                $"Cattle with Tag Number {request.TagNumber} already exists."));
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
    public async Task<Result> DeleteAsync(Guid id)
    {
        Cattle? cattle = await _context.Cattles.FindAsync(id);
        if (cattle is null)
        {
            return Result.Failure(Error.NotFound("Cattle.NotFound", $"Cattle with id {id} was not found."));
        }
        _context.Cattles.Remove(cattle);
        await _context.SaveChangesAsync();

        // Publish deletion event
        CattleDeleteEvent @event = new(id, DateTime.UtcNow);
        await _producer.ProduceAsync(id.ToString(), @event);
        
        return Result.Success();
    }
    public async Task<Result<Cattle>> UpdateAsync(Guid id, UpdateCattleRequest request)
    {
        Cattle? cattle = await _context.Cattles.FindAsync(id);
        if (cattle is null)
        {
            return Result.Failure<Cattle>(Error.NotFound("Cattle.NotFound", $"Cattle with id {id} was not found."));
        }
        if (cattle.TagNumber != request.TagNumber &&
            await _context.Cattles.AnyAsync(x => x.TagNumber == request.TagNumber))
        {
            return Result.Failure<Cattle>(Error.Conflict(
                "Cattle.TagNumberExists", 
                $"Cattle with Tag Number {request.TagNumber} already exists."));
        }
        cattle.Breed = request.Breed;
        cattle.TagNumber = request.TagNumber;
        cattle.DateOfBirth = request.DateOfBirth;
        _context.Cattles.Update(cattle);
        await _context.SaveChangesAsync();
        return cattle;
    }
}
