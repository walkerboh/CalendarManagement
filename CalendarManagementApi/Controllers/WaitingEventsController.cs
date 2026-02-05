using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalendarManagementApi.Data;
using CalendarManagementApi.DTOs;
using CalendarManagementApi.Models;

namespace CalendarManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WaitingEventsController : ControllerBase
{
    private readonly CalendarDbContext _context;
    private readonly ILogger<WaitingEventsController> _logger;

    public WaitingEventsController(CalendarDbContext context, ILogger<WaitingEventsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WaitingEventDto>>> GetAll()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var events = await _context.WaitingEvents
            .Select(e => new WaitingEventDto
            {
                Id = e.Id,
                Name = e.Name,
                OccurrenceDate = e.OccurrenceDate,
                IsPastDue = e.OccurrenceDate <= today,
                Layer = e.Layer
            })
            .ToListAsync();

        return Ok(events);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WaitingEventDto>> GetById(int id)
    {
        var waitingEvent = await _context.WaitingEvents.FindAsync(id);

        if (waitingEvent == null)
        {
            return NotFound();
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        return Ok(new WaitingEventDto
        {
            Id = waitingEvent.Id,
            Name = waitingEvent.Name,
            OccurrenceDate = waitingEvent.OccurrenceDate,
            IsPastDue = waitingEvent.OccurrenceDate <= today,
            Layer = waitingEvent.Layer
        });
    }

    [HttpPost]
    public async Task<ActionResult<WaitingEventDto>> Create(CreateWaitingEventDto dto)
    {
        var waitingEvent = new WaitingEvent
        {
            Name = dto.Name,
            OccurrenceDate = dto.OccurrenceDate,
            Layer = dto.Layer
        };

        _context.WaitingEvents.Add(waitingEvent);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created waiting event: {Name} on {Date}", waitingEvent.Name, waitingEvent.OccurrenceDate);

        var today = DateOnly.FromDateTime(DateTime.Today);
        var result = new WaitingEventDto
        {
            Id = waitingEvent.Id,
            Name = waitingEvent.Name,
            OccurrenceDate = waitingEvent.OccurrenceDate,
            IsPastDue = waitingEvent.OccurrenceDate <= today,
            Layer = waitingEvent.Layer
        };

        return CreatedAtAction(nameof(GetById), new { id = waitingEvent.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateWaitingEventDto dto)
    {
        var waitingEvent = await _context.WaitingEvents.FindAsync(id);

        if (waitingEvent == null)
        {
            return NotFound();
        }

        waitingEvent.Name = dto.Name;
        waitingEvent.OccurrenceDate = dto.OccurrenceDate;
        waitingEvent.Layer = dto.Layer;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated waiting event {Id}: {Name} on {Date}", id, waitingEvent.Name, waitingEvent.OccurrenceDate);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var waitingEvent = await _context.WaitingEvents.FindAsync(id);

        if (waitingEvent == null)
        {
            return NotFound();
        }

        _context.WaitingEvents.Remove(waitingEvent);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted waiting event {Id}: {Name}", id, waitingEvent.Name);

        return NoContent();
    }

    [HttpPost("{id}/postpone-week")]
    public async Task<IActionResult> PostponeWeek(int id)
    {
        var waitingEvent = await _context.WaitingEvents.FindAsync(id);

        if (waitingEvent == null)
        {
            return NotFound();
        }

        waitingEvent.OccurrenceDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7));
        await _context.SaveChangesAsync();

        _logger.LogInformation("Postponed waiting event {Id} by 1 week to {Date}", id, waitingEvent.OccurrenceDate);

        return NoContent();
    }

    [HttpPost("{id}/postpone-month")]
    public async Task<IActionResult> PostponeMonth(int id)
    {
        var waitingEvent = await _context.WaitingEvents.FindAsync(id);

        if (waitingEvent == null)
        {
            return NotFound();
        }

        waitingEvent.OccurrenceDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1));
        await _context.SaveChangesAsync();

        _logger.LogInformation("Postponed waiting event {Id} by 1 month to {Date}", id, waitingEvent.OccurrenceDate);

        return NoContent();
    }

    [HttpPost("{id}/postpone")]
    public async Task<IActionResult> PostponeToDate(int id, PostponeDateDto dto)
    {
        var waitingEvent = await _context.WaitingEvents.FindAsync(id);

        if (waitingEvent == null)
        {
            return NotFound();
        }

        waitingEvent.OccurrenceDate = dto.Date;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Postponed waiting event {Id} to {Date}", id, waitingEvent.OccurrenceDate);

        return NoContent();
    }
}
