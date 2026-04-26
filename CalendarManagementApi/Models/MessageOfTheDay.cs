using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CalendarManagement.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Layer
{
    Black,
    Red
}

public class MessageOfTheDay
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Message { get; set; } = string.Empty;

    [Range(1, 12)]
    public int Month { get; set; }

    [Range(1, 31)]
    public int Day { get; set; }

    [Required]
    public Layer Layer { get; set; } = Layer.Black;
}
