using Microsoft.AspNetCore.Mvc;
using CalendarManagementApi.DTOs;
using CalendarManagementApi.Models;
using CalendarManagementApi.Services;

namespace CalendarManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BirthdaysController : ControllerBase
{
    private readonly IBirthdayService _service;
    private readonly ILogger<BirthdaysController> _logger;

    public BirthdaysController(IBirthdayService service, ILogger<BirthdaysController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BirthdayDto>>> GetAll()
    {
        var birthdays = await _service.GetAllAsync();

        var result = birthdays.Select(e => new BirthdayDto
        {
            Id = e.Id,
            Name = e.Name,
            Month = e.Month,
            Day = e.Day
        });

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BirthdayDto>> GetById(int id)
    {
        var birthday = await _service.GetByIdAsync(id);

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

        await _service.CreateAsync(birthday);

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
        var birthday = new Birthday
        {
            Id = id,
            Name = dto.Name,
            Month = dto.Month,
            Day = dto.Day
        };

        var updated = await _service.UpdateAsync(birthday);

        if (!updated)
        {
            return NotFound();
        }

        _logger.LogInformation("Updated birthday {Id}: {Name} on {Month}/{Day}", id, birthday.Name, birthday.Month, birthday.Day);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        _logger.LogInformation("Deleted birthday {Id}", id);

        return NoContent();
    }
}
