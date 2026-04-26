using CalendarManagement.DTOs;

namespace CalendarManagement.Services;

public interface ICalendarService
{
    Task<List<CalendarEventDto>> GetEventsForDate(DateOnly date);
    Task<List<CalendarEventDto>> GetMotdForDate(DateOnly date);
    Task<List<BirthdayResponseDto>> GetBirthdaysForDate(DateOnly date);
}
