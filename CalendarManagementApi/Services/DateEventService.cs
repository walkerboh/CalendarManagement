using CalendarManagementApi.Data;
using CalendarManagementApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CalendarManagementApi.Services;

public class DateEventService : IDateEventService
{
    private readonly CalendarDbContext _context;

    public DateEventService(CalendarDbContext context)
    {
        _context = context;
    }

    public async Task<List<DateEvent>> GetAllAsync()
    {
        return await _context.DateEvents
            .OrderBy(e => e.Month)
            .ThenBy(e => e.Day)
            .ToListAsync();
    }

    public async Task<DateEvent?> GetByIdAsync(int id)
    {
        return await _context.DateEvents.FindAsync(id);
    }

    public async Task<DateEvent> CreateAsync(DateEvent dateEvent)
    {
        _context.DateEvents.Add(dateEvent);
        await _context.SaveChangesAsync();
        return dateEvent;
    }

    public async Task<bool> UpdateAsync(DateEvent dateEvent)
    {
        var existing = await _context.DateEvents.FindAsync(dateEvent.Id);
        if (existing == null)
            return false;

        existing.Name = dateEvent.Name;
        existing.Month = dateEvent.Month;
        existing.Day = dateEvent.Day;
        existing.TextColor = dateEvent.TextColor;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var dateEvent = await _context.DateEvents.FindAsync(id);
        if (dateEvent == null)
            return false;

        _context.DateEvents.Remove(dateEvent);
        await _context.SaveChangesAsync();
        return true;
    }
}
