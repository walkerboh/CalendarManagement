using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CalendarManagement.DTOs;
using CalendarManagement.Services;

namespace CalendarManagement.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CalendarController : ControllerBase
{
    private readonly ICalendarService _calendarService;
    private readonly ILogger<CalendarController> _logger;

    public CalendarController(ICalendarService calendarService, ILogger<CalendarController> logger)
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

    [HttpGet("motd/{date}")]
    public async Task<ActionResult<IEnumerable<CalendarEventDto>>> GetMotdForDate(DateOnly date)
    {
        _logger.LogInformation("Fetching messages of the day for date: {Date}", date);

        var events = await _calendarService.GetMotdForDate(date);

        _logger.LogInformation("Found {Count} messages of the day for date {Date}", events.Count, date);

        return Ok(events);
    }

    [HttpGet("birthdays/{date}")]
    public async Task<ActionResult<IEnumerable<BirthdayResponseDto>>> GetBirthdaysForDate(DateOnly date)
    {
        _logger.LogInformation("Fetching birthdays for date: {Date}", date);

        var birthdays = await _calendarService.GetBirthdaysForDate(date);

        _logger.LogInformation("Found {Count} birthdays within 14 days of {Date}", birthdays.Count, date);

        return Ok(birthdays);
    }
}
