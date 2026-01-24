using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CalendarManagementApi.Data;
using CalendarManagementApi.Models;

namespace CalendarManagementApi.Pages.DateEvents;

public class CreateModel : PageModel
{
    private readonly CalendarDbContext _context;

    public CreateModel(CalendarDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public DateEvent DateEvent { get; set; } = new DateEvent();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.DateEvents.Add(DateEvent);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
