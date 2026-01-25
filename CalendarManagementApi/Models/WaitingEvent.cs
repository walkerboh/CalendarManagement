using System.ComponentModel.DataAnnotations;

namespace CalendarManagementApi.Models;

public class WaitingEvent
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public DateOnly OccurrenceDate { get; set; }

    [MaxLength(100)]
    public string? Image { get; set; }
}
