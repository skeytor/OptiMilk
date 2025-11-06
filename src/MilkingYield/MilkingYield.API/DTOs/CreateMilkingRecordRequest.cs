using System.ComponentModel.DataAnnotations;

namespace MilkingYield.API.DTOs;

public sealed record CreateMilkingRecordRequest(
    [Required]
    Guid CowId,

    [Required,Range(0.1, double.MaxValue)]
    double YieldInLiters);
