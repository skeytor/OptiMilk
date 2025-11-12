using Microsoft.AspNetCore.Mvc;
using MilkingYield.API.DTOs;
using MilkingYield.API.Models;
using MilkingYield.API.Services;

namespace MilkingYield.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MilkingYieldController(MilkingSessionService service) : ControllerBase
{

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateAsync([FromBody] CreateMilkingRecordRequest request)
    {
        var result = await service.InsertAsync(request);
        return result.IsSuccess 
            ? CreatedAtAction(nameof(CreateAsync), result.Value) 
            : BadRequest(result.Error);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<GetMilkingRecord>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GetMilkingRecord>>> GetPagedAsync(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10) => await service.GetPagedRecordsAsync(page, size);

    [HttpGet("{cowId:guid}")]
    [ProducesResponseType(typeof(GetMilkingRecord), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetMilkingRecord>> GetByCowIdAsync([FromRoute] Guid cowId)
    {
        var result = await service.GetRecordByCowIdAsync(cowId);
        return result.IsSuccess 
            ? Ok(result.Value) 
            : NotFound(result.Error);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateMilkingSessionRequest request)
    {
        var result = await service.UpdateAsync(id, request);
        return result.IsSuccess 
            ? Ok(id) 
            : NotFound(result.Error);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> DeleteAsync([FromRoute] Guid id)
    {
        var result = await service.DeleteAsync(id);
        return result.IsSuccess 
            ? Ok(result.Value) 
            : NotFound(result.Error);
    }
}
