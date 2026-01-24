using CalendarManagementApi.Data;
using CalendarManagementApi.DTOs;
using CalendarManagementApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CalendarManagementApi.Services;

public class CalendarService
{
    private readonly CalendarDbContext _context;

    public CalendarService(CalendarDbContext context)
    {
        _context = context;
    }

    public async Task<List<CalendarEventDto>> GetEventsForDate(DateOnly date)
    {
        var events = new List<CalendarEventDto>();

        // Get waiting events that are due (occurrence date <= today)
        var today = DateOnly.FromDateTime(DateTime.Today);
        var waitingEvents = await _context.WaitingEvents
            .Where(e => e.OccurrenceDate <= today)
            .ToListAsync();

        events.AddRange(waitingEvents.Select(e => new CalendarEventDto
        {
            Name = e.Name,
            EventType = "WaitingEvent",
            SourceId = e.Id
        }));

        // Get repeating events that occur on this date
        var repeatingEvents = await _context.RepeatingEvents.ToListAsync();

        foreach (var repeatEvent in repeatingEvents)
        {
            if (DoesRepeatOnDate(repeatEvent, date))
            {
                events.Add(new CalendarEventDto
                {
                    Name = repeatEvent.Name,
                    EventType = "RepeatingEvent",
                    SourceId = repeatEvent.Id
                });
            }
        }

        return events;
    }

    public async Task<List<CalendarEventDto>> GetDateEventsForDate(DateOnly date)
    {
        var events = new List<CalendarEventDto>();

        // Get date events that match the month and day
        var dateEvents = await _context.DateEvents
            .Where(e => e.Month == date.Month && e.Day == date.Day)
            .ToListAsync();

        events.AddRange(dateEvents.Select(e => new CalendarEventDto
        {
            Name = e.Name,
            EventType = "DateEvent",
            SourceId = e.Id
        }));

        return events;
    }

    private bool DoesRepeatOnDate(RepeatingEvent repeatEvent, DateOnly date)
    {
        return repeatEvent.RepeatType switch
        {
            RepeatType.DayOfWeek => CheckDayOfWeek(repeatEvent, date),
            RepeatType.DayOfWeekOfMonth => CheckDayOfWeekOfMonth(repeatEvent, date),
            RepeatType.Interval => CheckInterval(repeatEvent, date),
            RepeatType.Date => CheckDate(repeatEvent, date),
            _ => false
        };
    }

    private bool CheckDate(RepeatingEvent repeatEvent, DateOnly date)
    {
        if (!repeatEvent.Month.HasValue || !repeatEvent.Day.HasValue)
            return false;
        return date.Month == repeatEvent.Month.Value && date.Day == repeatEvent.Day.Value;
    }

    private bool CheckDayOfWeek(RepeatingEvent repeatEvent, DateOnly date)
    {
        if (!repeatEvent.DayOfWeek.HasValue)
            return false;

        var dateDayOfWeek = (int)date.DayOfWeek;
        return dateDayOfWeek == repeatEvent.DayOfWeek.Value;
    }

    private bool CheckDayOfWeekOfMonth(RepeatingEvent repeatEvent, DateOnly date)
    {
        if (!repeatEvent.DayOfWeek.HasValue || string.IsNullOrEmpty(repeatEvent.WeeksOfMonth))
            return false;

        var dateDayOfWeek = (int)date.DayOfWeek;
        if (dateDayOfWeek != repeatEvent.DayOfWeek.Value)
            return false;

        // Calculate which week of the month this date falls on
        var weekOfMonth = GetWeekOfMonth(date);

        // Parse weeks from the comma-separated string
        var weeks = repeatEvent.WeeksOfMonth.Split(',')
            .Select(w => int.TryParse(w.Trim(), out var week) ? week : 0)
            .Where(w => w > 0)
            .ToList();

        return weeks.Contains(weekOfMonth);
    }

    private int GetWeekOfMonth(DateOnly date)
    {
        // Week 1 = days 1-7, Week 2 = days 8-14, etc.
        return ((date.Day - 1) / 7) + 1;
    }

    private bool CheckInterval(RepeatingEvent repeatEvent, DateOnly date)
    {
        if (!repeatEvent.IntervalDays.HasValue || !repeatEvent.StartDate.HasValue)
            return false;

        if (date < repeatEvent.StartDate.Value)
            return false;

        var daysSinceStart = date.DayNumber - repeatEvent.StartDate.Value.DayNumber;
        return daysSinceStart % repeatEvent.IntervalDays.Value == 0;
    }
}
