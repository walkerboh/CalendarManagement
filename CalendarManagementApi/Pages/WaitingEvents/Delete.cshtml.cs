using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CalendarManagementApi.Data;
using CalendarManagementApi.Models;

namespace CalendarManagementApi.Pages.WaitingEvents;

public class DeleteModel : PageModel
{
    private readonly CalendarDbContext _context;

    public DeleteModel(CalendarDbContext context)
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
        var waitingEvent = await _context.WaitingEvents.FindAsync(WaitingEvent.Id);

        if (waitingEvent == null)
        {
            return NotFound();
        }

        _context.WaitingEvents.Remove(waitingEvent);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
