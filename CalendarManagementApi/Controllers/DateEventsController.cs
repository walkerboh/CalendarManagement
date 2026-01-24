using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalendarManagementApi.Data;
using CalendarManagementApi.DTOs;
using CalendarManagementApi.Models;

namespace CalendarManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DateEventsController : ControllerBase
{
    private readonly CalendarDbContext _context;
    private readonly ILogger<DateEventsController> _logger;

    public DateEventsController(CalendarDbContext context, ILogger<DateEventsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DateEventDto>>> GetAll()
    {
        var events = await _context.DateEvents
            .Select(e => new DateEventDto
            {
                Id = e.Id,
                Name = e.Name,
                Month = e.Month,
                Day = e.Day
            })
            .ToListAsync();

        return Ok(events);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DateEventDto>> GetById(int id)
    {
        var dateEvent = await _context.DateEvents.FindAsync(id);

        if (dateEvent == null)
        {
            return NotFound();
        }

        return Ok(new DateEventDto
        {
            Id = dateEvent.Id,
            Name = dateEvent.Name,
            Month = dateEvent.Month,
            Day = dateEvent.Day
        });
    }

    [HttpPost]
    public async Task<ActionResult<DateEventDto>> Create(CreateDateEventDto dto)
    {
        var dateEvent = new DateEvent
        {
            Name = dto.Name,
            Month = dto.Month,
            Day = dto.Day
        };

        _context.DateEvents.Add(dateEvent);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created date event: {Name} on {Month}/{Day}", dateEvent.Name, dateEvent.Month, dateEvent.Day);

        var result = new DateEventDto
        {
            Id = dateEvent.Id,
            Name = dateEvent.Name,
            Month = dateEvent.Month,
            Day = dateEvent.Day
        };

        return CreatedAtAction(nameof(GetById), new { id = dateEvent.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateDateEventDto dto)
    {
        var dateEvent = await _context.DateEvents.FindAsync(id);

        if (dateEvent == null)
        {
            return NotFound();
        }

        dateEvent.Name = dto.Name;
        dateEvent.Month = dto.Month;
        dateEvent.Day = dto.Day;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated date event {Id}: {Name} on {Month}/{Day}", id, dateEvent.Name, dateEvent.Month, dateEvent.Day);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var dateEvent = await _context.DateEvents.FindAsync(id);

        if (dateEvent == null)
        {
            return NotFound();
        }

        _context.DateEvents.Remove(dateEvent);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted date event {Id}: {Name}", id, dateEvent.Name);

        return NoContent();
    }
}
