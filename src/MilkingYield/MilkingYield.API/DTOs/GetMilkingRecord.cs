using MilkingYield.API.Models;

namespace MilkingYield.API.DTOs;

public sealed record GetMilkingRecord(
    Guid Id,
    Cattle Cattle,
    DateTime RecordedAt,
    double YieldInLiters);
