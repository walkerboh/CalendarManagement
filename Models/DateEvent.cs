using System.ComponentModel.DataAnnotations;

namespace CalendarManagementApi.Models;

public class DateEvent
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Range(1, 12)]
    public int Month { get; set; }

    [Range(1, 31)]
    public int Day { get; set; }
}
