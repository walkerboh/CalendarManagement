using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CalendarManagementApi.DTOs;
using CalendarManagementApi.Models;
using CalendarManagementApi.Services;

namespace CalendarManagementApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RepeatingEventsController : ControllerBase
{
    private readonly IRepeatingEventService _service;
    private readonly ILogger<RepeatingEventsController> _logger;

    public RepeatingEventsController(IRepeatingEventService service, ILogger<RepeatingEventsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RepeatingEventDto>>> GetAll()
    {
        var events = await _service.GetAllAsync();

        var result = events.Select(e => new RepeatingEventDto
        {
            Id = e.Id,
            Name = e.Name,
            RepeatType = e.RepeatType,
            DayOfWeek = e.DayOfWeek,
            WeeksOfMonth = e.WeeksOfMonth,
            IntervalDays = e.IntervalDays,
            StartDate = e.StartDate,
            Month = e.Month,
            Day = e.Day,
            Layer = e.Layer
        });

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RepeatingEventDto>> GetById(int id)
    {
        var repeatingEvent = await _service.GetByIdAsync(id);

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
            StartDate = repeatingEvent.StartDate,
            Month = repeatingEvent.Month,
            Day = repeatingEvent.Day,
            Layer = repeatingEvent.Layer
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
            StartDate = dto.StartDate,
            Month = dto.Month,
            Day = dto.Day,
            Layer = dto.Layer
        };

        await _service.CreateAsync(repeatingEvent);

        _logger.LogInformation("Created repeating event: {Name} with type {Type}", repeatingEvent.Name, repeatingEvent.RepeatType);

        var result = new RepeatingEventDto
        {
            Id = repeatingEvent.Id,
            Name = repeatingEvent.Name,
            RepeatType = repeatingEvent.RepeatType,
            DayOfWeek = repeatingEvent.DayOfWeek,
            WeeksOfMonth = repeatingEvent.WeeksOfMonth,
            IntervalDays = repeatingEvent.IntervalDays,
            StartDate = repeatingEvent.StartDate,
            Month = repeatingEvent.Month,
            Day = repeatingEvent.Day,
            Layer = repeatingEvent.Layer
        };

        return CreatedAtAction(nameof(GetById), new { id = repeatingEvent.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateRepeatingEventDto dto)
    {
        var repeatingEvent = new RepeatingEvent
        {
            Id = id,
            Name = dto.Name,
            RepeatType = dto.RepeatType,
            DayOfWeek = dto.DayOfWeek,
            WeeksOfMonth = dto.WeeksOfMonth,
            IntervalDays = dto.IntervalDays,
            StartDate = dto.StartDate,
            Month = dto.Month,
            Day = dto.Day,
            Layer = dto.Layer
        };

        var updated = await _service.UpdateAsync(repeatingEvent);

        if (!updated)
        {
            return NotFound();
        }

        _logger.LogInformation("Updated repeating event {Id}: {Name} with type {Type}", id, repeatingEvent.Name, repeatingEvent.RepeatType);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        _logger.LogInformation("Deleted repeating event {Id}", id);

        return NoContent();
    }
}
