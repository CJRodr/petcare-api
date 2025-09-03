using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetcareApi.Data;
using PetcareApi.Models;

namespace PetcareApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PetsController : ControllerBase
{
    private readonly AppDbContext _db;
    public PetsController(AppDbContext db) => _db = db;

    // GET /pets
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] int? ownerId = null)
    {
        var query = _db.Pets.AsQueryable();
        if (ownerId is not null) query = query.Where(p => p.OwnerId == ownerId);

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new {
                p.Id, p.Name, p.Species, p.OwnerId
            })
            .ToListAsync();

        return Ok(new { items, page, pageSize, total });
    }

    // GET /pets/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<object>> GetOne(int id)
    {
        var p = await _db.Pets.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return p is null ? NotFound() : Ok(new { p.Id, p.Name, p.Species, p.OwnerId });
    }

    // POST /pets
    [HttpPost]
    public async Task<ActionResult<object>> Create([FromBody] CreatePetDto body)
    {
        if (string.IsNullOrWhiteSpace(body.Name) || string.IsNullOrWhiteSpace(body.Species))
            return ValidationProblem("Name and Species are required.");

        var pet = new Pet { Name = body.Name, Species = body.Species, OwnerId = body.OwnerId };
        _db.Pets.Add(pet);
        await _db.SaveChangesAsync();
        return Created($"/pets/{pet.Id}", new { pet.Id, pet.Name, pet.Species, pet.OwnerId });
    }

    // PUT /pets/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdatePetDto body)
    {
        var pet = await _db.Pets.FindAsync(id);
        if (pet is null) return NotFound();

        if (!string.IsNullOrWhiteSpace(body.Name)) pet.Name = body.Name!;
        if (!string.IsNullOrWhiteSpace(body.Species)) pet.Species = body.Species!;
        pet.OwnerId = body.OwnerId;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /pets/5
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var pet = await _db.Pets.FindAsync(id);
        if (pet is null) return NotFound();
        _db.Pets.Remove(pet);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

public record CreatePetDto(string Name, string Species, int? OwnerId);
public record UpdatePetDto(string? Name, string? Species, int? OwnerId);
