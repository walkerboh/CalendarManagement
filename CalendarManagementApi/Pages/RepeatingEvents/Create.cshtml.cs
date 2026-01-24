using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CalendarManagementApi.Data;
using CalendarManagementApi.Models;

namespace CalendarManagementApi.Pages.RepeatingEvents;

public class CreateModel : PageModel
{
    private readonly CalendarDbContext _context;

    public CreateModel(CalendarDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public RepeatingEvent RepeatingEvent { get; set; } = new RepeatingEvent();

    public void OnGet()
    {
        RepeatingEvent.StartDate = DateOnly.FromDateTime(DateTime.Today);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.RepeatingEvents.Add(RepeatingEvent);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
