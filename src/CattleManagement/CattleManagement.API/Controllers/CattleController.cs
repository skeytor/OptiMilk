using CattleManagement.API.DTOs;
using CattleManagement.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CattleManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CattleController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Cattle>>> GetAll() => await context.Cattles.ToListAsync();

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Cattle?>> GetById([FromRoute] Guid id)
    {
        Cattle? cattle = await context.Cattles.FindAsync(id);
        return cattle is not null ? cattle : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<Cattle>> CreateCattle([FromBody] CreateCattleRequest request)
    {
        
        context.Cattles.Add(cattle);
        await context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = cattle.Id }, cattle);
    }
}
