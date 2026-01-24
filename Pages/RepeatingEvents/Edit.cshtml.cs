using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CalendarManagementApi.Data;
using CalendarManagementApi.Models;

namespace CalendarManagementApi.Pages.RepeatingEvents;

public class EditModel : PageModel
{
    private readonly CalendarDbContext _context;

    public EditModel(CalendarDbContext context)
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
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var repeatingEvent = await _context.RepeatingEvents.FindAsync(RepeatingEvent.Id);

        if (repeatingEvent == null)
        {
            return NotFound();
        }

        repeatingEvent.Name = RepeatingEvent.Name;
        repeatingEvent.RepeatType = RepeatingEvent.RepeatType;
        repeatingEvent.DayOfWeek = RepeatingEvent.DayOfWeek;
        repeatingEvent.WeeksOfMonth = RepeatingEvent.WeeksOfMonth;
        repeatingEvent.IntervalDays = RepeatingEvent.IntervalDays;
        repeatingEvent.StartDate = RepeatingEvent.StartDate;

        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
