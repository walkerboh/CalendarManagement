using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CalendarManagementApi.Data;
using CalendarManagementApi.DTOs;
using CalendarManagementApi.Models;
using CalendarManagementApi.Services;

namespace CalendarManagementApi.Pages;

public class IndexModel : PageModel
{
    private readonly CalendarService _calendarService;
    private readonly CalendarDbContext _context;

    public IndexModel(CalendarService calendarService, CalendarDbContext context)
    {
        _calendarService = calendarService;
        _context = context;
    }

    public List<CalendarEventDto> TodaysEvents { get; set; } = new();
    public List<WaitingEvent> DueWaitingEvents { get; set; } = new();
    public DateOnly Today { get; set; }

    public async Task OnGetAsync()
    {
        Today = DateOnly.FromDateTime(DateTime.Today);
        TodaysEvents = await _calendarService.GetEventsForDate(Today);

        DueWaitingEvents = await _context.WaitingEvents
            .Where(e => e.OccurrenceDate <= Today)
            .OrderBy(e => e.OccurrenceDate)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostPostponeWeekAsync(int id)
    {
        var waitingEvent = await _context.WaitingEvents.FindAsync(id);

        if (waitingEvent == null)
        {
            return NotFound();
        }

        waitingEvent.OccurrenceDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7));
        await _context.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostPostponeMonthAsync(int id)
    {
        var waitingEvent = await _context.WaitingEvents.FindAsync(id);

        if (waitingEvent == null)
        {
            return NotFound();
        }

        waitingEvent.OccurrenceDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1));
        await _context.SaveChangesAsync();

        return RedirectToPage();
    }
}
