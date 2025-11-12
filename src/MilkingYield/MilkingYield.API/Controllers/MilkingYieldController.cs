using Microsoft.AspNetCore.Mvc;
using MilkingYield.API.DTOs;
using MilkingYield.API.Models;
using MilkingYield.API.Services;

namespace MilkingYield.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MilkingYieldController(AppDbContext contex, MilkingSessionService service) : ControllerBase
{

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMilkingRecordRequest request)
    {
        MilkingRecord milkingRecord = new()
        {
            CowId = request.CowId,
            YieldInLiters = request.YieldInLiters
        };
        await contex.MilkingYields.AddAsync(milkingRecord);
        await contex.SaveChangesAsync();
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<List<GetMilkingRecord>>> GetPaged(int page = 1, int size = 10)
    {
        return await service.GetMilkingSessionsAsync(page, size);
    }
}
