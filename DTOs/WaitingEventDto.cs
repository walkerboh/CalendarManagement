using System.ComponentModel.DataAnnotations;

namespace CalendarManagementApi.DTOs;

public class WaitingEventDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly OccurrenceDate { get; set; }
    public bool IsPastDue { get; set; }
}

public class CreateWaitingEventDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public DateOnly OccurrenceDate { get; set; }
}

public class UpdateWaitingEventDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public DateOnly OccurrenceDate { get; set; }
}
