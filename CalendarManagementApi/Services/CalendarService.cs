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
            SourceId = e.Id,
            Image = e.Image,
            Layer = e.Layer
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
                    SourceId = repeatEvent.Id,
                    Image = repeatEvent.Image,
                    Layer = repeatEvent.Layer
                });
            }
        }

        return events;
    }

    public async Task<List<CalendarEventDto>> GetMotdForDate(DateOnly date)
    {
        var events = new List<CalendarEventDto>();

        // Get messages of the day that match the month and day
        var messages = await _context.MessagesOfTheDay
            .Where(e => e.Month == date.Month && e.Day == date.Day)
            .ToListAsync();

        events.AddRange(messages.Select(e => new CalendarEventDto
        {
            Name = e.Message,
            EventType = "MessageOfTheDay",
            SourceId = e.Id,
            Layer = e.Layer
        }));

        return events;
    }

    public async Task<List<BirthdayResponseDto>> GetBirthdaysForDate(DateOnly date)
    {
        // Generate all (month, day) tuples within the 14-day window
        var dateRange = Enumerable.Range(0, 15)
            .Select(i => date.AddDays(i))
            .Select(d => (d.Month, d.Day))
            .ToHashSet();

        // Fetch all birthdays and filter in memory (EF Core can't translate tuple Contains to SQL)
        var allBirthdays = await _context.Birthdays.ToListAsync();
        var birthdays = allBirthdays.Where(b => dateRange.Contains((b.Month, b.Day)));

        return birthdays.Select(b => new BirthdayResponseDto
        {
            Id = b.Id,
            Name = b.Name,
            Month = b.Month,
            Day = b.Day,
            DaysAway = CalculateDaysAway(date, b.Month, b.Day)
        }).OrderBy(b => b.DaysAway).ToList();
    }

    internal int CalculateDaysAway(DateOnly fromDate, int month, int day)
    {
        // Find the next occurrence of this birthday from the given date
        var targetYear = fromDate.Year;
        var targetDate = new DateOnly(targetYear, month, day);

        // If the target date is before fromDate, it must be next year
        if (targetDate < fromDate)
        {
            targetDate = new DateOnly(targetYear + 1, month, day);
        }

        return targetDate.DayNumber - fromDate.DayNumber;
    }

    internal bool DoesRepeatOnDate(RepeatingEvent repeatEvent, DateOnly date)
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

    internal bool CheckDate(RepeatingEvent repeatEvent, DateOnly date)
    {
        if (!repeatEvent.Month.HasValue || !repeatEvent.Day.HasValue)
            return false;
        return date.Month == repeatEvent.Month.Value && date.Day == repeatEvent.Day.Value;
    }

    internal bool CheckDayOfWeek(RepeatingEvent repeatEvent, DateOnly date)
    {
        if (!repeatEvent.DayOfWeek.HasValue)
            return false;

        var dateDayOfWeek = (int)date.DayOfWeek;
        return dateDayOfWeek == repeatEvent.DayOfWeek.Value;
    }

    internal bool CheckDayOfWeekOfMonth(RepeatingEvent repeatEvent, DateOnly date)
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

    internal int GetWeekOfMonth(DateOnly date)
    {
        // Week 1 = days 1-7, Week 2 = days 8-14, etc.
        return ((date.Day - 1) / 7) + 1;
    }

    internal bool CheckInterval(RepeatingEvent repeatEvent, DateOnly date)
    {
        if (!repeatEvent.IntervalDays.HasValue || !repeatEvent.StartDate.HasValue)
            return false;

        if (date < repeatEvent.StartDate.Value)
            return false;

        var daysSinceStart = date.DayNumber - repeatEvent.StartDate.Value.DayNumber;
        return daysSinceStart % repeatEvent.IntervalDays.Value == 0;
    }
}
