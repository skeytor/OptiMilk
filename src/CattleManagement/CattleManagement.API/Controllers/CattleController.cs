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
    public async Task<ActionResult<Cattle?>> GetById(Guid id)
    {
        Cattle? cattle = await context.Cattles.FindAsync(id);
        return cattle is not null ? cattle : NotFound();
    }
}
