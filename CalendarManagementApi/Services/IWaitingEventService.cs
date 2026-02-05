using CalendarManagementApi.Models;

namespace CalendarManagementApi.Services;

public interface IWaitingEventService
{
    Task<List<WaitingEvent>> GetAllAsync();
    Task<List<WaitingEvent>> GetDueEventsAsync();
    Task<WaitingEvent?> GetByIdAsync(int id);
    Task<WaitingEvent> CreateAsync(WaitingEvent waitingEvent);
    Task<bool> UpdateAsync(WaitingEvent waitingEvent);
    Task<bool> DeleteAsync(int id);
    Task<bool> PostponeWeekAsync(int id);
    Task<bool> PostponeMonthAsync(int id);
    Task<bool> PostponeToDateAsync(int id, DateOnly date);
}
