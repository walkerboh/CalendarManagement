using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CalendarManagement.DTOs;
using CalendarManagement.Models;
using CalendarManagement.Services;

namespace CalendarManagement.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MessageOfTheDayController : ControllerBase
{
    private readonly IMessageOfTheDayService _service;
    private readonly ILogger<MessageOfTheDayController> _logger;

    public MessageOfTheDayController(IMessageOfTheDayService service, ILogger<MessageOfTheDayController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageOfTheDayDto>>> GetAll()
    {
        var messages = await _service.GetAllAsync();

        var result = messages.Select(e => new MessageOfTheDayDto
        {
            Id = e.Id,
            Message = e.Message,
            Month = e.Month,
            Day = e.Day,
            Layer = e.Layer
        });

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MessageOfTheDayDto>> GetById(int id)
    {
        var motd = await _service.GetByIdAsync(id);

        if (motd == null)
        {
            return NotFound();
        }

        return Ok(new MessageOfTheDayDto
        {
            Id = motd.Id,
            Message = motd.Message,
            Month = motd.Month,
            Day = motd.Day,
            Layer = motd.Layer
        });
    }

    [HttpPost]
    public async Task<ActionResult<MessageOfTheDayDto>> Create(CreateMessageOfTheDayDto dto)
    {
        var motd = new MessageOfTheDay
        {
            Message = dto.Message,
            Month = dto.Month,
            Day = dto.Day,
            Layer = dto.Layer
        };

        try
        {
            await _service.CreateAsync(motd);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }

        _logger.LogInformation("Created message of the day: {Message} on {Month}/{Day}", motd.Message, motd.Month, motd.Day);

        var result = new MessageOfTheDayDto
        {
            Id = motd.Id,
            Message = motd.Message,
            Month = motd.Month,
            Day = motd.Day,
            Layer = motd.Layer
        };

        return CreatedAtAction(nameof(GetById), new { id = motd.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateMessageOfTheDayDto dto)
    {
        var motd = new MessageOfTheDay
        {
            Id = id,
            Message = dto.Message,
            Month = dto.Month,
            Day = dto.Day,
            Layer = dto.Layer
        };

        try
        {
            var updated = await _service.UpdateAsync(motd);
            if (!updated)
            {
                return NotFound();
            }
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }

        _logger.LogInformation("Updated message of the day {Id}: {Message} on {Month}/{Day}", id, motd.Message, motd.Month, motd.Day);

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

        _logger.LogInformation("Deleted message of the day {Id}", id);

        return NoContent();
    }
}
