using CalendarManagementApi.Data;
using CalendarManagementApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CalendarManagementApi.Services;

public class WaitingEventService : IWaitingEventService
{
    private readonly CalendarDbContext _context;
    private readonly IDateProvider _dateProvider;
    private readonly ILogger<WaitingEventService> _logger;

    public WaitingEventService(CalendarDbContext context, IDateProvider dateProvider, ILogger<WaitingEventService> logger)
    {
        _context = context;
        _dateProvider = dateProvider;
        _logger = logger;
    }

    public async Task<List<WaitingEvent>> GetAllAsync()
    {
        return await _context.WaitingEvents
            .OrderBy(e => e.OccurrenceDate)
            .ToListAsync();
    }

    public async Task<List<WaitingEvent>> GetDueEventsAsync()
    {
        var today = _dateProvider.Today;
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
        _logger.LogInformation("Created waiting event {Id}: {Name} on {Date}", waitingEvent.Id, waitingEvent.Name, waitingEvent.OccurrenceDate);
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
        existing.Layer = waitingEvent.Layer;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated waiting event {Id}: {Name} on {Date}", waitingEvent.Id, waitingEvent.Name, waitingEvent.OccurrenceDate);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var waitingEvent = await _context.WaitingEvents.FindAsync(id);
        if (waitingEvent == null)
            return false;

        _context.WaitingEvents.Remove(waitingEvent);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Deleted waiting event {Id}: {Name}", id, waitingEvent.Name);
        return true;
    }

    public async Task<bool> PostponeWeekAsync(int id)
    {
        var newDate = _dateProvider.Today.AddDays(7);
        return await PostponeToDateAsync(id, newDate);
    }

    public async Task<bool> PostponeMonthAsync(int id)
    {
        var newDate = _dateProvider.Today.AddMonths(1);
        return await PostponeToDateAsync(id, newDate);
    }

    public async Task<bool> PostponeToDateAsync(int id, DateOnly date)
    {
        var waitingEvent = await _context.WaitingEvents.FindAsync(id);
        if (waitingEvent == null)
            return false;

        waitingEvent.OccurrenceDate = date;
        await _context.SaveChangesAsync();
        _logger.LogInformation("Postponed waiting event {Id} to {Date}", id, date);
        return true;
    }
}
