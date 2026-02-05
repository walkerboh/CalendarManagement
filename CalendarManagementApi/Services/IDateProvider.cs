namespace CalendarManagementApi.Services;

public interface IDateProvider
{
    DateOnly Today { get; }
}
