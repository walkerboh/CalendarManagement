using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CalendarManagementApi.Data;
using CalendarManagementApi.Models;

namespace CalendarManagementApi.Pages.RepeatingEvents;

public class DeleteModel : PageModel
{
    private readonly CalendarDbContext _context;

    public DeleteModel(CalendarDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public RepeatingEvent RepeatingEvent { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var repeatingEvent = await _context.RepeatingEvents.FindAsync(id);

        if (repeatingEvent == null)
        {
            return NotFound();
        }

        RepeatingEvent = repeatingEvent;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var repeatingEvent = await _context.RepeatingEvents.FindAsync(RepeatingEvent.Id);

        if (repeatingEvent == null)
        {
            return NotFound();
        }

        _context.RepeatingEvents.Remove(repeatingEvent);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
