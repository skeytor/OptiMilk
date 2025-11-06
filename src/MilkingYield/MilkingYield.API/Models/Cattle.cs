namespace MilkingYield.API.Models;

public sealed class Cattle
{
    public Guid Id { get; set; }
    public string TagNumber { get; set; } = null!;
    public string Breed { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
}
