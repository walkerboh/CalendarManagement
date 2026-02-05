using Microsoft.AspNetCore.Mvc;
using CalendarManagementApi.DTOs;
using CalendarManagementApi.Models;
using CalendarManagementApi.Services;

namespace CalendarManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WaitingEventsController : ControllerBase
{
    private readonly IWaitingEventService _service;
    private readonly IDateProvider _dateProvider;
    private readonly ILogger<WaitingEventsController> _logger;

    public WaitingEventsController(IWaitingEventService service, IDateProvider dateProvider, ILogger<WaitingEventsController> logger)
    {
        _service = service;
        _dateProvider = dateProvider;
        _logger = logger;
    }

    private bool IsPastDue(DateOnly occurrenceDate) => occurrenceDate <= _dateProvider.Today;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WaitingEventDto>>> GetAll()
    {
        var events = await _service.GetAllAsync();

        var result = events.Select(e => new WaitingEventDto
        {
            Id = e.Id,
            Name = e.Name,
            OccurrenceDate = e.OccurrenceDate,
            IsPastDue = IsPastDue(e.OccurrenceDate),
            Layer = e.Layer
        });

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WaitingEventDto>> GetById(int id)
    {
        var waitingEvent = await _service.GetByIdAsync(id);

        if (waitingEvent == null)
        {
            return NotFound();
        }

        return Ok(new WaitingEventDto
        {
            Id = waitingEvent.Id,
            Name = waitingEvent.Name,
            OccurrenceDate = waitingEvent.OccurrenceDate,
            IsPastDue = IsPastDue(waitingEvent.OccurrenceDate),
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

        await _service.CreateAsync(waitingEvent);

        _logger.LogInformation("Created waiting event: {Name} on {Date}", waitingEvent.Name, waitingEvent.OccurrenceDate);

        var result = new WaitingEventDto
        {
            Id = waitingEvent.Id,
            Name = waitingEvent.Name,
            OccurrenceDate = waitingEvent.OccurrenceDate,
            IsPastDue = IsPastDue(waitingEvent.OccurrenceDate),
            Layer = waitingEvent.Layer
        };

        return CreatedAtAction(nameof(GetById), new { id = waitingEvent.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateWaitingEventDto dto)
    {
        var waitingEvent = new WaitingEvent
        {
            Id = id,
            Name = dto.Name,
            OccurrenceDate = dto.OccurrenceDate,
            Layer = dto.Layer
        };

        var updated = await _service.UpdateAsync(waitingEvent);

        if (!updated)
        {
            return NotFound();
        }

        _logger.LogInformation("Updated waiting event {Id}: {Name} on {Date}", id, waitingEvent.Name, waitingEvent.OccurrenceDate);

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

        _logger.LogInformation("Deleted waiting event {Id}", id);

        return NoContent();
    }

    [HttpPost("{id}/postpone-week")]
    public async Task<IActionResult> PostponeWeek(int id)
    {
        var postponed = await _service.PostponeWeekAsync(id);

        if (!postponed)
        {
            return NotFound();
        }

        _logger.LogInformation("Postponed waiting event {Id} by 1 week", id);

        return NoContent();
    }

    [HttpPost("{id}/postpone-month")]
    public async Task<IActionResult> PostponeMonth(int id)
    {
        var postponed = await _service.PostponeMonthAsync(id);

        if (!postponed)
        {
            return NotFound();
        }

        _logger.LogInformation("Postponed waiting event {Id} by 1 month", id);

        return NoContent();
    }

    [HttpPost("{id}/postpone")]
    public async Task<IActionResult> PostponeToDate(int id, PostponeDateDto dto)
    {
        var postponed = await _service.PostponeToDateAsync(id, dto.Date);

        if (!postponed)
        {
            return NotFound();
        }

        _logger.LogInformation("Postponed waiting event {Id} to {Date}", id, dto.Date);

        return NoContent();
    }
}
