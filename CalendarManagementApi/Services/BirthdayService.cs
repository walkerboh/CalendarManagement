using CalendarManagementApi.Data;
using CalendarManagementApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CalendarManagementApi.Services;

public class BirthdayService : IBirthdayService
{
    private readonly CalendarDbContext _context;
    private readonly ILogger<BirthdayService> _logger;

    public BirthdayService(CalendarDbContext context, ILogger<BirthdayService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Birthday>> GetAllAsync()
    {
        return await _context.Birthdays
            .OrderBy(e => e.Month)
            .ThenBy(e => e.Day)
            .ToListAsync();
    }

    public async Task<Birthday?> GetByIdAsync(int id)
    {
        return await _context.Birthdays.FindAsync(id);
    }

    public async Task<Birthday> CreateAsync(Birthday birthday)
    {
        _context.Birthdays.Add(birthday);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Created birthday {Id}: {Name} on {Month}/{Day}", birthday.Id, birthday.Name, birthday.Month, birthday.Day);
        return birthday;
    }

    public async Task<bool> UpdateAsync(Birthday birthday)
    {
        var existingEntity = await _context.Birthdays.FindAsync(birthday.Id);
        if (existingEntity == null)
            return false;

        existingEntity.Name = birthday.Name;
        existingEntity.Month = birthday.Month;
        existingEntity.Day = birthday.Day;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated birthday {Id}: {Name} on {Month}/{Day}", birthday.Id, birthday.Name, birthday.Month, birthday.Day);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var birthday = await _context.Birthdays.FindAsync(id);
        if (birthday == null)
            return false;

        _context.Birthdays.Remove(birthday);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Deleted birthday {Id}: {Name}", id, birthday.Name);
        return true;
    }
}
