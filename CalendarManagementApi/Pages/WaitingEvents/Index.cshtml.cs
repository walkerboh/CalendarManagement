using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CalendarManagementApi.Data;
using CalendarManagementApi.Models;

namespace CalendarManagementApi.Pages.WaitingEvents;

public class IndexModel : PageModel
{
    private readonly CalendarDbContext _context;

    public IndexModel(CalendarDbContext context)
    {
        _context = context;
    }

    public IList<WaitingEvent> WaitingEvents { get; set; } = new List<WaitingEvent>();

    public async Task OnGetAsync()
    {
        WaitingEvents = await _context.WaitingEvents
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
