using Microsoft.AspNetCore.Mvc;
using CalendarManagementApi.DTOs;
using CalendarManagementApi.Services;

namespace CalendarManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CalendarController : ControllerBase
{
    private readonly CalendarService _calendarService;
    private readonly ILogger<CalendarController> _logger;

    public CalendarController(CalendarService calendarService, ILogger<CalendarController> logger)
    {
        _calendarService = calendarService;
        _logger = logger;
    }

    [HttpGet("{date}")]
    public async Task<ActionResult<IEnumerable<CalendarEventDto>>> GetEventsForDate(DateOnly date)
    {
        _logger.LogInformation("Fetching events for date: {Date}", date);

        var events = await _calendarService.GetEventsForDate(date);

        _logger.LogInformation("Found {Count} events for date {Date}", events.Count, date);

        return Ok(events);
    }

    [HttpGet("dates/{date}")]
    public async Task<ActionResult<IEnumerable<CalendarEventDto>>> GetDateEventsForDate(DateOnly date)
    {
        _logger.LogInformation("Fetching date events for date: {Date}", date);

        var events = await _calendarService.GetDateEventsForDate(date);

        _logger.LogInformation("Found {Count} date events for date {Date}", events.Count, date);

        return Ok(events);
    }
}
