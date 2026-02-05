using System.ComponentModel.DataAnnotations;

namespace CalendarManagementApi.Models;

public enum RepeatType
{
    DayOfWeek,
    DayOfWeekOfMonth,
    Interval,
    Date
}

public class RepeatingEvent
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public RepeatType RepeatType { get; set; }

    // For DayOfWeek and DayOfWeekOfMonth types (0=Sunday to 6=Saturday)
    public int? DayOfWeek { get; set; }

    // For DayOfWeekOfMonth type - comma-separated weeks: "1,3" for 1st and 3rd week
    [MaxLength(20)]
    public string? WeeksOfMonth { get; set; }

    // For Interval type
    public int? IntervalDays { get; set; }

    // For Interval type - the starting date for interval calculation
    public DateOnly? StartDate { get; set; }

    // For Date type - yearly on specific month/day
    public int? Month { get; set; }  // 1-12
    public int? Day { get; set; }    // 1-31

    [MaxLength(100)]
    public string? Image { get; set; }

    [Required]
    public Layer Layer { get; set; } = Layer.Black;
}
