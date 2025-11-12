using Microsoft.EntityFrameworkCore;
using MilkingYield.API.Clients;
using MilkingYield.API.DTOs;
using MilkingYield.API.Models;
using SharedKernel.Results;

namespace MilkingYield.API.Services;

public sealed class MilkingSessionService(CattleApiClient client, AppDbContext context)
{
    private readonly CattleApiClient _client = client;
    private readonly AppDbContext _context = context;
    public async Task<Result<MilkingRecord>> InsertAsync(CreateMilkingRecordRequest request)
    {
        if (!await _client.CattleExistsAsync(request.CowId))
        {
            return Result.Failure<MilkingRecord>(Error.NotFound(
                "Cattle.NotFound",
                $"Cattle with ID {request.CowId} does not exist"));
        }
        MilkingRecord milkingRecord = new()
        {
            CowId = request.CowId,
            YieldInLiters = request.YieldInLiters
        };
        await _context.MilkingYields.AddAsync(milkingRecord);
        await _context.SaveChangesAsync();
        return Result.Success(milkingRecord);
    }
    public async Task<List<GetMilkingRecord>> GetPagedRecordsAsync(int page, int size)
    {
        List<MilkingRecord> milkingRecords = await _context.MilkingYields
            .AsNoTracking()
            .OrderByDescending(m => m.Date)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        List<Task<Cattle?>> fetchTasks = [.. milkingRecords.Select(record => _client.GetCowByIdAsync(record.CowId))];
        List<GetMilkingRecord> results = [];
        Cattle?[] response = await Task.WhenAll(fetchTasks);

        foreach (var cattle in response)
        {
            if (cattle is null)
            {
                continue;
            }
            var record = milkingRecords.First(m => m.CowId == cattle.Id);
            results.Add(new GetMilkingRecord(record.Id, cattle, record.Date, record.YieldInLiters));
        }
        return results;
    }
    
    public async Task<Result<GetMilkingRecord>> GetRecordByCowIdAsync(Guid cowId)
    {
        if (await _client.GetCowByIdAsync(cowId) is not Cattle cow)
        {
            return Result.Failure<GetMilkingRecord>(Error.NotFound(
                "Cattle.NotFound",
                $"Cattle with ID {cowId} does not exist"));
        }
        MilkingRecord? record = await _context.MilkingYields
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.CowId == cowId);
        if (record is null)
        {
            return Result.Failure<GetMilkingRecord>(Error.NotFound(
                "MilkingRecord.NotFound",
                $"Milking record for Cattle ID {cowId} does not exist"));
        }
        return Result.Success(new GetMilkingRecord(
            record.Id,
            cow,
            record.Date,
            record.YieldInLiters));
    }
    public async Task<Result<Guid>> DeleteAsync(Guid milkingRecordId)
    {
        MilkingRecord? record = await _context.MilkingYields
            .FirstOrDefaultAsync(m => m.Id == milkingRecordId);
        if (record is null)
        {
            return Result.Failure<Guid>(Error.NotFound(
                "MilkingRecord.NotFound",
                $"Milking record with ID {milkingRecordId} does not exist"));
        }
        _context.MilkingYields.Remove(record);
        await _context.SaveChangesAsync();
        return Result.Success(milkingRecordId);
    }
    public async Task<Result> UpdateAsync(Guid milkingRecordId, UpdateMilkingSessionRequest request)
    {
        MilkingRecord? record = await _context.MilkingYields
            .FirstOrDefaultAsync(m => m.Id == milkingRecordId);
        if (record is null)
        {
            return Result.Failure(Error.NotFound(
                "MilkingRecord.NotFound",
                $"Milking record with ID {milkingRecordId} does not exist"));
        }
        if (!await _client.CattleExistsAsync(request.CowId))
        {
            return Result.Failure(Error.NotFound(
                "Cattle.NotFound",
                $"Cattle with ID {request.CowId} does not exist"));
        }
        record.CowId = request.CowId;
        record.YieldInLiters = request.YieldInLiters;
        await _context.SaveChangesAsync();
        return Result.Success();
    }
}
