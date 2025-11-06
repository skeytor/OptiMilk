namespace MilkingYield.API.Models;

public sealed class MilkingRecord
{
    public Guid Id { get; set; }
    public Guid CowId { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public double YieldInLiters { get; set; }
}
