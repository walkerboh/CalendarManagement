using CalendarManagement.Models;

namespace CalendarManagement.Services;

public interface IRepeatingEventService
{
    Task<List<RepeatingEvent>> GetAllAsync();
    Task<RepeatingEvent?> GetByIdAsync(int id);
    Task<RepeatingEvent> CreateAsync(RepeatingEvent repeatingEvent);
    Task<bool> UpdateAsync(RepeatingEvent repeatingEvent);
    Task<bool> DeleteAsync(int id);
}
