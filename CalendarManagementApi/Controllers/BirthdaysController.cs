using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalendarManagementApi.Data;
using CalendarManagementApi.DTOs;
using CalendarManagementApi.Models;

namespace CalendarManagementApi.Controllers;

[ApiController]
[Route("api/birthdays")]
public class BirthdaysController : ControllerBase
{
    private readonly CalendarDbContext _context;
    private readonly ILogger<BirthdaysController> _logger;

    public BirthdaysController(CalendarDbContext context, ILogger<BirthdaysController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BirthdayDto>>> GetAll()
    {
        var birthdays = await _context.Birthdays
            .Select(e => new BirthdayDto
            {
                Id = e.Id,
                Name = e.Name,
                Month = e.Month,
                Day = e.Day
            })
            .ToListAsync();

        return Ok(birthdays);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BirthdayDto>> GetById(int id)
    {
        var birthday = await _context.Birthdays.FindAsync(id);

        if (birthday == null)
        {
            return NotFound();
        }

        return Ok(new BirthdayDto
        {
            Id = birthday.Id,
            Name = birthday.Name,
            Month = birthday.Month,
            Day = birthday.Day
        });
    }

    [HttpPost]
    public async Task<ActionResult<BirthdayDto>> Create(CreateBirthdayDto dto)
    {
        var birthday = new Birthday
        {
            Name = dto.Name,
            Month = dto.Month,
            Day = dto.Day
        };

        _context.Birthdays.Add(birthday);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created birthday: {Name} on {Month}/{Day}", birthday.Name, birthday.Month, birthday.Day);

        var result = new BirthdayDto
        {
            Id = birthday.Id,
            Name = birthday.Name,
            Month = birthday.Month,
            Day = birthday.Day
        };

        return CreatedAtAction(nameof(GetById), new { id = birthday.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateBirthdayDto dto)
    {
        var birthday = await _context.Birthdays.FindAsync(id);

        if (birthday == null)
        {
            return NotFound();
        }

        birthday.Name = dto.Name;
        birthday.Month = dto.Month;
        birthday.Day = dto.Day;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated birthday {Id}: {Name} on {Month}/{Day}", id, birthday.Name, birthday.Month, birthday.Day);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var birthday = await _context.Birthdays.FindAsync(id);

        if (birthday == null)
        {
            return NotFound();
        }

        _context.Birthdays.Remove(birthday);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted birthday {Id}: {Name}", id, birthday.Name);

        return NoContent();
    }
}
