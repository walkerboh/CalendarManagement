namespace CalendarManagement.DTOs;

public class BirthdayResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Month { get; set; }
    public int Day { get; set; }
    public int DaysAway { get; set; }
}
