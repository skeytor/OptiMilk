using MilkingYield.API.Models;

namespace MilkingYield.API.Data;

internal static class SampleData
{
    internal static MilkingRecord[] MilkingYields =>
    [
        new() {
            Id = Guid.NewGuid(),
            CowId = Guid.Parse("935da9aa-7610-4876-ba98-452ff109baba"),
            Date = DateTime.UtcNow.Date.AddDays(-3),
            YieldInLiters = 15.5
        },
        new() {
            Id = Guid.NewGuid(),
            CowId = Guid.Parse("e49e5c17-c74c-43c2-bc96-55a3bae5712b"),
            Date = DateTime.UtcNow.Date.AddDays(-3),
            YieldInLiters = 10.5
        },
        new() {
            Id = Guid.NewGuid(),
            CowId = Guid.Parse("e49e5c17-c74c-43c2-bc96-55a3bae5712b"),
            Date = DateTime.UtcNow.Date.AddDays(-3),
            YieldInLiters = 13.5
        },
        new() {
            Id = Guid.NewGuid(),
            CowId = Guid.Parse("069f9cc8-d3bd-47b3-8dc2-7e343c90fe30"),
            Date = DateTime.UtcNow.Date.AddDays(-3),
            YieldInLiters = 12.5
        },
    ];
}
