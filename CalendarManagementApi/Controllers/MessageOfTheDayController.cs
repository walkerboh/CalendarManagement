using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalendarManagementApi.Data;
using CalendarManagementApi.DTOs;
using CalendarManagementApi.Models;

namespace CalendarManagementApi.Controllers;

[ApiController]
[Route("api/messageoftheday")]
public class MessageOfTheDayController : ControllerBase
{
    private readonly CalendarDbContext _context;
    private readonly ILogger<MessageOfTheDayController> _logger;

    public MessageOfTheDayController(CalendarDbContext context, ILogger<MessageOfTheDayController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageOfTheDayDto>>> GetAll()
    {
        var messages = await _context.MessagesOfTheDay
            .Select(e => new MessageOfTheDayDto
            {
                Id = e.Id,
                Message = e.Message,
                Month = e.Month,
                Day = e.Day,
                Layer = e.Layer
            })
            .ToListAsync();

        return Ok(messages);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MessageOfTheDayDto>> GetById(int id)
    {
        var motd = await _context.MessagesOfTheDay.FindAsync(id);

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

        var existing = await _context.MessagesOfTheDay
            .AnyAsync(e => e.Month == dto.Month && e.Day == dto.Day);
        if (existing)
        {
            return Conflict($"A message of the day already exists for {dto.Month}/{dto.Day}.");
        }

        _context.MessagesOfTheDay.Add(motd);
        await _context.SaveChangesAsync();

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
        var motd = await _context.MessagesOfTheDay.FindAsync(id);

        if (motd == null)
        {
            return NotFound();
        }

        var duplicate = await _context.MessagesOfTheDay
            .AnyAsync(e => e.Month == dto.Month && e.Day == dto.Day && e.Id != id);
        if (duplicate)
        {
            return Conflict($"A message of the day already exists for {dto.Month}/{dto.Day}.");
        }

        motd.Message = dto.Message;
        motd.Month = dto.Month;
        motd.Day = dto.Day;
        motd.Layer = dto.Layer;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated message of the day {Id}: {Message} on {Month}/{Day}", id, motd.Message, motd.Month, motd.Day);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var motd = await _context.MessagesOfTheDay.FindAsync(id);

        if (motd == null)
        {
            return NotFound();
        }

        _context.MessagesOfTheDay.Remove(motd);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted message of the day {Id}: {Message}", id, motd.Message);

        return NoContent();
    }
}
