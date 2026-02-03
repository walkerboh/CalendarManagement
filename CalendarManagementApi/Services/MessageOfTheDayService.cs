using CalendarManagementApi.Data;
using CalendarManagementApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CalendarManagementApi.Services;

public class MessageOfTheDayService : IMessageOfTheDayService
{
    private readonly CalendarDbContext _context;

    public MessageOfTheDayService(CalendarDbContext context)
    {
        _context = context;
    }

    public async Task<List<MessageOfTheDay>> GetAllAsync()
    {
        return await _context.MessagesOfTheDay
            .OrderBy(e => e.Month)
            .ThenBy(e => e.Day)
            .ToListAsync();
    }

    public async Task<MessageOfTheDay?> GetByIdAsync(int id)
    {
        return await _context.MessagesOfTheDay.FindAsync(id);
    }

    public async Task<MessageOfTheDay> CreateAsync(MessageOfTheDay messageOfTheDay)
    {
        var existing = await _context.MessagesOfTheDay
            .AnyAsync(e => e.Month == messageOfTheDay.Month && e.Day == messageOfTheDay.Day);
        if (existing)
            throw new InvalidOperationException($"A message of the day already exists for {messageOfTheDay.Month}/{messageOfTheDay.Day}.");

        _context.MessagesOfTheDay.Add(messageOfTheDay);
        await _context.SaveChangesAsync();
        return messageOfTheDay;
    }

    public async Task<bool> UpdateAsync(MessageOfTheDay messageOfTheDay)
    {
        var existingEntity = await _context.MessagesOfTheDay.FindAsync(messageOfTheDay.Id);
        if (existingEntity == null)
            return false;

        var duplicate = await _context.MessagesOfTheDay
            .AnyAsync(e => e.Month == messageOfTheDay.Month && e.Day == messageOfTheDay.Day && e.Id != messageOfTheDay.Id);
        if (duplicate)
            throw new InvalidOperationException($"A message of the day already exists for {messageOfTheDay.Month}/{messageOfTheDay.Day}.");

        existingEntity.Message = messageOfTheDay.Message;
        existingEntity.Month = messageOfTheDay.Month;
        existingEntity.Day = messageOfTheDay.Day;
        existingEntity.TextColor = messageOfTheDay.TextColor;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var messageOfTheDay = await _context.MessagesOfTheDay.FindAsync(id);
        if (messageOfTheDay == null)
            return false;

        _context.MessagesOfTheDay.Remove(messageOfTheDay);
        await _context.SaveChangesAsync();
        return true;
    }
}
