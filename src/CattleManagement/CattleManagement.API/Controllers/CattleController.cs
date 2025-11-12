using CattleManagement.API.DTOs;
using CattleManagement.API.Models;
using CattleManagement.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CattleManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CattleController(CattleService cattleService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<Cattle>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Cattle>>> GetAll() => await cattleService.GetAllAsync();

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Cattle), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Cattle?>> GetById([FromRoute] Guid id)
    {
        var result = await cattleService.GetCattleByIdAsync(id);
        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound();
    }

    [HttpPost]
    [ProducesResponseType(typeof(Cattle), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Cattle>> CreateCattle([FromBody] CreateCattleRequest request)
    {
        var result = await cattleService.InsertAsync(request);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
            : BadRequest(result.Error);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Cattle), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Cattle>> UpdateCattle(
        [FromRoute] Guid id, 
        [FromBody] UpdateCattleRequest request)
    {
        var result = await cattleService.UpdateAsync(id, request);
        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCattle([FromRoute] Guid id)
    {
        var result = await cattleService.DeleteAsync(id);
        return result.IsSuccess
            ? NoContent()
            : BadRequest(result.Error);
    }

}
