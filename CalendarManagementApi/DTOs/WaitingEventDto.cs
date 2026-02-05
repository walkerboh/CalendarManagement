using System.ComponentModel.DataAnnotations;
using CalendarManagementApi.Models;

namespace CalendarManagementApi.DTOs;

public class WaitingEventDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly OccurrenceDate { get; set; }
    public bool IsPastDue { get; set; }
    public Layer Layer { get; set; }
}

public class CreateWaitingEventDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public DateOnly OccurrenceDate { get; set; }

    public Layer Layer { get; set; }
}

public class UpdateWaitingEventDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public DateOnly OccurrenceDate { get; set; }

    public Layer Layer { get; set; }
}
