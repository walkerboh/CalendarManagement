using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalendarManagementApi.Data;
using CalendarManagementApi.DTOs;
using CalendarManagementApi.Models;

namespace CalendarManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RepeatingEventsController : ControllerBase
{
    private readonly CalendarDbContext _context;
    private readonly ILogger<RepeatingEventsController> _logger;

    public RepeatingEventsController(CalendarDbContext context, ILogger<RepeatingEventsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RepeatingEventDto>>> GetAll()
    {
        var events = await _context.RepeatingEvents
            .Select(e => new RepeatingEventDto
            {
                Id = e.Id,
                Name = e.Name,
                RepeatType = e.RepeatType,
                DayOfWeek = e.DayOfWeek,
                WeeksOfMonth = e.WeeksOfMonth,
                IntervalDays = e.IntervalDays,
                StartDate = e.StartDate
            })
            .ToListAsync();

        return Ok(events);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RepeatingEventDto>> GetById(int id)
    {
        var repeatingEvent = await _context.RepeatingEvents.FindAsync(id);

        if (repeatingEvent == null)
        {
            return NotFound();
        }

        return Ok(new RepeatingEventDto
        {
            Id = repeatingEvent.Id,
            Name = repeatingEvent.Name,
            RepeatType = repeatingEvent.RepeatType,
            DayOfWeek = repeatingEvent.DayOfWeek,
            WeeksOfMonth = repeatingEvent.WeeksOfMonth,
            IntervalDays = repeatingEvent.IntervalDays,
            StartDate = repeatingEvent.StartDate
        });
    }

    [HttpPost]
    public async Task<ActionResult<RepeatingEventDto>> Create(CreateRepeatingEventDto dto)
    {
        var repeatingEvent = new RepeatingEvent
        {
            Name = dto.Name,
            RepeatType = dto.RepeatType,
            DayOfWeek = dto.DayOfWeek,
            WeeksOfMonth = dto.WeeksOfMonth,
            IntervalDays = dto.IntervalDays,
            StartDate = dto.StartDate
        };

        _context.RepeatingEvents.Add(repeatingEvent);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created repeating event: {Name} with type {Type}", repeatingEvent.Name, repeatingEvent.RepeatType);

        var result = new RepeatingEventDto
        {
            Id = repeatingEvent.Id,
            Name = repeatingEvent.Name,
            RepeatType = repeatingEvent.RepeatType,
            DayOfWeek = repeatingEvent.DayOfWeek,
            WeeksOfMonth = repeatingEvent.WeeksOfMonth,
            IntervalDays = repeatingEvent.IntervalDays,
            StartDate = repeatingEvent.StartDate
        };

        return CreatedAtAction(nameof(GetById), new { id = repeatingEvent.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateRepeatingEventDto dto)
    {
        var repeatingEvent = await _context.RepeatingEvents.FindAsync(id);

        if (repeatingEvent == null)
        {
            return NotFound();
        }

        repeatingEvent.Name = dto.Name;
        repeatingEvent.RepeatType = dto.RepeatType;
        repeatingEvent.DayOfWeek = dto.DayOfWeek;
        repeatingEvent.WeeksOfMonth = dto.WeeksOfMonth;
        repeatingEvent.IntervalDays = dto.IntervalDays;
        repeatingEvent.StartDate = dto.StartDate;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated repeating event {Id}: {Name} with type {Type}", id, repeatingEvent.Name, repeatingEvent.RepeatType);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var repeatingEvent = await _context.RepeatingEvents.FindAsync(id);

        if (repeatingEvent == null)
        {
            return NotFound();
        }

        _context.RepeatingEvents.Remove(repeatingEvent);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted repeating event {Id}: {Name}", id, repeatingEvent.Name);

        return NoContent();
    }
}
