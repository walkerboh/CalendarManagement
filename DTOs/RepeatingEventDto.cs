using System.ComponentModel.DataAnnotations;
using CalendarManagementApi.Models;

namespace CalendarManagementApi.DTOs;

public class RepeatingEventDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public RepeatType RepeatType { get; set; }
    public int? DayOfWeek { get; set; }
    public string? WeeksOfMonth { get; set; }
    public int? IntervalDays { get; set; }
    public DateOnly? StartDate { get; set; }
}

public class CreateRepeatingEventDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public RepeatType RepeatType { get; set; }

    [Range(0, 6)]
    public int? DayOfWeek { get; set; }

    [MaxLength(20)]
    public string? WeeksOfMonth { get; set; }

    [Range(1, int.MaxValue)]
    public int? IntervalDays { get; set; }

    public DateOnly? StartDate { get; set; }
}

public class UpdateRepeatingEventDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public RepeatType RepeatType { get; set; }

    [Range(0, 6)]
    public int? DayOfWeek { get; set; }

    [MaxLength(20)]
    public string? WeeksOfMonth { get; set; }

    [Range(1, int.MaxValue)]
    public int? IntervalDays { get; set; }

    public DateOnly? StartDate { get; set; }
}
