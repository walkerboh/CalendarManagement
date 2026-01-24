using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CalendarManagementApi.Data;
using CalendarManagementApi.Models;

namespace CalendarManagementApi.Pages.WaitingEvents;

public class EditModel : PageModel
{
    private readonly CalendarDbContext _context;

    public EditModel(CalendarDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public WaitingEvent WaitingEvent { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var waitingEvent = await _context.WaitingEvents.FindAsync(id);

        if (waitingEvent == null)
        {
            return NotFound();
        }

        WaitingEvent = waitingEvent;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var waitingEvent = await _context.WaitingEvents.FindAsync(WaitingEvent.Id);

        if (waitingEvent == null)
        {
            return NotFound();
        }

        waitingEvent.Name = WaitingEvent.Name;
        waitingEvent.OccurrenceDate = WaitingEvent.OccurrenceDate;

        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
