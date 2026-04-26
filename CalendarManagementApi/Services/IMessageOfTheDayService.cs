using CalendarManagement.Models;

namespace CalendarManagement.Services;

public interface IMessageOfTheDayService
{
    Task<List<MessageOfTheDay>> GetAllAsync();
    Task<MessageOfTheDay?> GetByIdAsync(int id);
    Task<MessageOfTheDay> CreateAsync(MessageOfTheDay messageOfTheDay);
    Task<bool> UpdateAsync(MessageOfTheDay messageOfTheDay);
    Task<bool> DeleteAsync(int id);
}
