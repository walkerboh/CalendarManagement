using System.ComponentModel.DataAnnotations;
using CalendarManagement.Models;

namespace CalendarManagement.DTOs;

public class MessageOfTheDayDto
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public int Month { get; set; }
    public int Day { get; set; }
    public Layer Layer { get; set; }
}

public class CreateMessageOfTheDayDto
{
    [Required]
    [MaxLength(200)]
    public string Message { get; set; } = string.Empty;

    [Range(1, 12)]
    public int Month { get; set; }

    [Range(1, 31)]
    public int Day { get; set; }

    public Layer Layer { get; set; }
}

public class UpdateMessageOfTheDayDto
{
    [Required]
    [MaxLength(200)]
    public string Message { get; set; } = string.Empty;

    [Range(1, 12)]
    public int Month { get; set; }

    [Range(1, 31)]
    public int Day { get; set; }

    public Layer Layer { get; set; }
}
