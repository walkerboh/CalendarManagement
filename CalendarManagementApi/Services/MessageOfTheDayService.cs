using CalendarManagementApi.Data;
using CalendarManagementApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CalendarManagementApi.Services;

public class MessageOfTheDayService : IMessageOfTheDayService
{
    private readonly CalendarDbContext _context;
    private readonly ILogger<MessageOfTheDayService> _logger;

    public MessageOfTheDayService(CalendarDbContext context, ILogger<MessageOfTheDayService> logger)
    {
        _context = context;
        _logger = logger;
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
        _logger.LogInformation("Created message of the day {Id}: {Message} on {Month}/{Day}", messageOfTheDay.Id, messageOfTheDay.Message, messageOfTheDay.Month, messageOfTheDay.Day);
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
        existingEntity.Layer = messageOfTheDay.Layer;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated message of the day {Id}: {Message} on {Month}/{Day}", messageOfTheDay.Id, messageOfTheDay.Message, messageOfTheDay.Month, messageOfTheDay.Day);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var messageOfTheDay = await _context.MessagesOfTheDay.FindAsync(id);
        if (messageOfTheDay == null)
            return false;

        _context.MessagesOfTheDay.Remove(messageOfTheDay);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Deleted message of the day {Id}: {Message}", id, messageOfTheDay.Message);
        return true;
    }
}
