namespace CalendarManagement.Services;

public interface IDateProvider
{
    DateOnly Today { get; }
}
