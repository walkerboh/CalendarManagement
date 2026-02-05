using CalendarManagementApi.Data;
using CalendarManagementApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CalendarManagementApi.Services;

public class RepeatingEventService : IRepeatingEventService
{
    private readonly CalendarDbContext _context;

    public RepeatingEventService(CalendarDbContext context)
    {
        _context = context;
    }

    public async Task<List<RepeatingEvent>> GetAllAsync()
    {
        return await _context.RepeatingEvents
            .OrderBy(e => e.Name)
            .ToListAsync();
    }

    public async Task<RepeatingEvent?> GetByIdAsync(int id)
    {
        return await _context.RepeatingEvents.FindAsync(id);
    }

    public async Task<RepeatingEvent> CreateAsync(RepeatingEvent repeatingEvent)
    {
        _context.RepeatingEvents.Add(repeatingEvent);
        await _context.SaveChangesAsync();
        return repeatingEvent;
    }

    public async Task<bool> UpdateAsync(RepeatingEvent repeatingEvent)
    {
        var existing = await _context.RepeatingEvents.FindAsync(repeatingEvent.Id);
        if (existing == null)
            return false;

        existing.Name = repeatingEvent.Name;
        existing.RepeatType = repeatingEvent.RepeatType;
        existing.DayOfWeek = repeatingEvent.DayOfWeek;
        existing.WeeksOfMonth = repeatingEvent.WeeksOfMonth;
        existing.IntervalDays = repeatingEvent.IntervalDays;
        existing.StartDate = repeatingEvent.StartDate;
        existing.Month = repeatingEvent.Month;
        existing.Day = repeatingEvent.Day;
        existing.Image = repeatingEvent.Image;
        existing.Layer = repeatingEvent.Layer;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var repeatingEvent = await _context.RepeatingEvents.FindAsync(id);
        if (repeatingEvent == null)
            return false;

        _context.RepeatingEvents.Remove(repeatingEvent);
        await _context.SaveChangesAsync();
        return true;
    }
}
