using CalendarManagementApi.Data;
using CalendarManagementApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CalendarManagementApi.Services;

public class BirthdayService : IBirthdayService
{
    private readonly CalendarDbContext _context;

    public BirthdayService(CalendarDbContext context)
    {
        _context = context;
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
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var birthday = await _context.Birthdays.FindAsync(id);
        if (birthday == null)
            return false;

        _context.Birthdays.Remove(birthday);
        await _context.SaveChangesAsync();
        return true;
    }
}
