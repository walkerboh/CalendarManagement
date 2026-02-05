using System.ComponentModel.DataAnnotations;

namespace CalendarManagementApi.DTOs;

public class BirthdayDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Month { get; set; }
    public int Day { get; set; }
}

public class CreateBirthdayDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Range(1, 12)]
    public int Month { get; set; }

    [Range(1, 31)]
    public int Day { get; set; }
}

public class UpdateBirthdayDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Range(1, 12)]
    public int Month { get; set; }

    [Range(1, 31)]
    public int Day { get; set; }
}
