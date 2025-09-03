using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetcareApi.Data;
using PetcareApi.Models;

namespace PetcareApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly AppDbContext _db;
    public AppointmentsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> Get([FromQuery] int? petId = null)
    {
        var q = _db.Appointments.AsQueryable();
        if (petId is not null) q = q.Where(a => a.PetId == petId);

        var list = await q.OrderBy(a => a.When)
            .Select(a => new {
                a.Id, a.PetId, a.When, a.Reason
            }).ToListAsync();

        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<object>> GetOne(int id)
    {
        var a = await _db.Appointments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return a is null ? NotFound() : Ok(new { a.Id, a.PetId, a.When, a.Reason });
    }

    [HttpPost]
    public async Task<ActionResult<object>> Create([FromBody] CreateAppointmentDto body)
    {
        // Basic validation: pet must exist
        var petExists = await _db.Pets.AnyAsync(p => p.Id == body.PetId);
        if (!petExists) return ValidationProblem($"Pet {body.PetId} does not exist.");

        var appt = new Appointment { PetId = body.PetId, When = body.When, Reason = body.Reason ?? "" };
        _db.Appointments.Add(appt);
        await _db.SaveChangesAsync();
        return Created($"/appointments/{appt.Id}", new { appt.Id, appt.PetId, appt.When, appt.Reason });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateAppointmentDto body)
    {
        var appt = await _db.Appointments.FindAsync(id);
        if (appt is null) return NotFound();
        if (body.When is not null) appt.When = body.When.Value;
        if (body.Reason is not null) appt.Reason = body.Reason;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var appt = await _db.Appointments.FindAsync(id);
        if (appt is null) return NotFound();
        _db.Appointments.Remove(appt);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

public record CreateAppointmentDto(int PetId, DateTime When, string? Reason);
public record UpdateAppointmentDto(DateTime? When, string? Reason);
