using System.ComponentModel.DataAnnotations;

namespace CalendarManagement.Models;

public class WaitingEvent
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public DateOnly OccurrenceDate { get; set; }

    [MaxLength(100)]
    public string? Image { get; set; }

    [Required]
    public Layer Layer { get; set; } = Layer.Black;
}
