using Microsoft.EntityFrameworkCore;
using MilkingYield.API.Clients;
using MilkingYield.API.DTOs;
using MilkingYield.API.Models;

namespace MilkingYield.API.Services;

public class MilkingSessionService(CattleApiClient client, AppDbContext context)
{
    public readonly CattleApiClient _client = client;
    public async Task<bool> CreateMilkingRecord(CreateMilkingRecordRequest request)
    {
        if (!await _client.CattleExistsAsync(request.CowId))
        {
            throw new ArgumentException("Cattle does not exist.");
        }
        MilkingRecord milkingRecord = new()
        {
            CowId = request.CowId,
            YieldInLiters = request.YieldInLiters
        };
        await context.MilkingYields.AddAsync(milkingRecord);
        await context.SaveChangesAsync();
        return true;
    }
    public async Task<List<GetMilkingRecord>> GetMilkingRecordsAsync(int page, int size)
    {
        List<MilkingRecord> milkingRecords = await context.MilkingYields
            .AsNoTracking()
            .OrderByDescending(m => m.Date)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();
        List<Task<Cattle?>> tasks = [.. milkingRecords.Select(record => _client.GetCowByIdAsync(record.CowId))];
        List<GetMilkingRecord> results = [];
        Cattle?[] response = await Task.WhenAll(tasks);
        foreach (var cattle in response)
        {
            if (cattle is null) continue;
            var record = milkingRecords.First(m => m.CowId == cattle.Id);
            results.Add(new GetMilkingRecord(record.Id, cattle, record.Date, record.YieldInLiters));
        }
        return results;
    }
}
