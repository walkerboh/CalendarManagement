using CalendarManagementApi.Models;

namespace CalendarManagementApi.DTOs;

public class CalendarEventDto
{
    public string Name { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public int SourceId { get; set; }
    public TextColor? TextColor { get; set; }
    public string? Image { get; set; }
}
