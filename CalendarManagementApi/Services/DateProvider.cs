namespace CalendarManagementApi.Services;

public class DateProvider : IDateProvider
{
    public DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
}
