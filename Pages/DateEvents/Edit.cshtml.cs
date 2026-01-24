using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CalendarManagementApi.Data;
using CalendarManagementApi.Models;

namespace CalendarManagementApi.Pages.DateEvents;

public class EditModel : PageModel
{
    private readonly CalendarDbContext _context;

    public EditModel(CalendarDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public DateEvent DateEvent { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var dateEvent = await _context.DateEvents.FindAsync(id);

        if (dateEvent == null)
        {
            return NotFound();
        }

        DateEvent = dateEvent;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var dateEvent = await _context.DateEvents.FindAsync(DateEvent.Id);

        if (dateEvent == null)
        {
            return NotFound();
        }

        dateEvent.Name = DateEvent.Name;
        dateEvent.Month = DateEvent.Month;
        dateEvent.Day = DateEvent.Day;

        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
