using CalendarManagementApi.Data;
using CalendarManagementApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CalendarManagementApi.Services;

public class WaitingEventService : IWaitingEventService
{
    private readonly CalendarDbContext _context;

    public WaitingEventService(CalendarDbContext context)
    {
        _context = context;
    }

    public async Task<List<WaitingEvent>> GetAllAsync()
    {
        return await _context.WaitingEvents
            .OrderBy(e => e.OccurrenceDate)
            .ToListAsync();
    }

    public async Task<List<WaitingEvent>> GetDueEventsAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        return await _context.WaitingEvents
            .Where(e => e.OccurrenceDate <= today)
            .OrderBy(e => e.OccurrenceDate)
            .ToListAsync();
    }

    public async Task<WaitingEvent?> GetByIdAsync(int id)
    {
        return await _context.WaitingEvents.FindAsync(id);
    }

    public async Task<WaitingEvent> CreateAsync(WaitingEvent waitingEvent)
    {
        _context.WaitingEvents.Add(waitingEvent);
        await _context.SaveChangesAsync();
        return waitingEvent;
    }

    public async Task<bool> UpdateAsync(WaitingEvent waitingEvent)
    {
        var existing = await _context.WaitingEvents.FindAsync(waitingEvent.Id);
        if (existing == null)
            return false;

        existing.Name = waitingEvent.Name;
        existing.OccurrenceDate = waitingEvent.OccurrenceDate;
        existing.Image = waitingEvent.Image;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var waitingEvent = await _context.WaitingEvents.FindAsync(id);
        if (waitingEvent == null)
            return false;

        _context.WaitingEvents.Remove(waitingEvent);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> PostponeWeekAsync(int id)
    {
        var waitingEvent = await _context.WaitingEvents.FindAsync(id);
        if (waitingEvent == null)
            return false;

        waitingEvent.OccurrenceDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7));
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> PostponeMonthAsync(int id)
    {
        var waitingEvent = await _context.WaitingEvents.FindAsync(id);
        if (waitingEvent == null)
            return false;

        waitingEvent.OccurrenceDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1));
        await _context.SaveChangesAsync();
        return true;
    }
}
