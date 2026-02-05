using CalendarManagementApi.DTOs;

namespace CalendarManagementApi.Services;

public interface ICalendarService
{
    Task<List<CalendarEventDto>> GetEventsForDate(DateOnly date);
    Task<List<CalendarEventDto>> GetMotdForDate(DateOnly date);
    Task<List<BirthdayResponseDto>> GetBirthdaysForDate(DateOnly date);
}
