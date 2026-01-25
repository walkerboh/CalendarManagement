using CalendarManagementApi.Models;

namespace CalendarManagementApi.Services;

public interface IDateEventService
{
    Task<List<DateEvent>> GetAllAsync();
    Task<DateEvent?> GetByIdAsync(int id);
    Task<DateEvent> CreateAsync(DateEvent dateEvent);
    Task<bool> UpdateAsync(DateEvent dateEvent);
    Task<bool> DeleteAsync(int id);
}
