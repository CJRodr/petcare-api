using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetcareApi.Data;
using PetcareApi.Models;

namespace PetcareApi.Controllers;

[ApiController]
[Route("[controller]")]
public class OwnersController : ControllerBase
{
    private readonly AppDbContext _db;
    public OwnersController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> Get()
    {
        var owners = await _db.Owners
            .OrderBy(o => o.Id)
            .Select(o => new { o.Id, o.Name, o.Email, petCount = o.Pets.Count })
            .ToListAsync();
        return Ok(owners);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<object>> GetOne(int id)
    {
        var o = await _db.Owners
            .Include(x => x.Pets)
            .FirstOrDefaultAsync(x => x.Id == id);
        return o is null
            ? NotFound()
            : Ok(new {
                o.Id, o.Name, o.Email,
                pets = o.Pets.Select(p => new { p.Id, p.Name, p.Species })
            });
    }

    [HttpPost]
    public async Task<ActionResult<object>> Create([FromBody] CreateOwnerDto body)
    {
        if (string.IsNullOrWhiteSpace(body.Name) || string.IsNullOrWhiteSpace(body.Email))
            return ValidationProblem("Name and Email are required.");
        var owner = new Owner { Name = body.Name, Email = body.Email };
        _db.Owners.Add(owner);
        await _db.SaveChangesAsync();
        return Created($"/owners/{owner.Id}", new { owner.Id, owner.Name, owner.Email });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateOwnerDto body)
    {
        var owner = await _db.Owners.FindAsync(id);
        if (owner is null) return NotFound();
        if (!string.IsNullOrWhiteSpace(body.Name)) owner.Name = body.Name!;
        if (!string.IsNullOrWhiteSpace(body.Email)) owner.Email = body.Email!;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var owner = await _db.Owners.FindAsync(id);
        if (owner is null) return NotFound();
        _db.Owners.Remove(owner);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

public record CreateOwnerDto(string Name, string Email);
public record UpdateOwnerDto(string? Name, string? Email);
