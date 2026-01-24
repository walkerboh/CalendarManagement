using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CalendarManagementApi.Data;
using CalendarManagementApi.Models;

namespace CalendarManagementApi.Pages.WaitingEvents;

public class CreateModel : PageModel
{
    private readonly CalendarDbContext _context;

    public CreateModel(CalendarDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public WaitingEvent WaitingEvent { get; set; } = new WaitingEvent();

    public void OnGet()
    {
        WaitingEvent.OccurrenceDate = DateOnly.FromDateTime(DateTime.Today);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.WaitingEvents.Add(WaitingEvent);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
