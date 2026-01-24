using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CalendarManagementApi.Data;
using CalendarManagementApi.Models;

namespace CalendarManagementApi.Pages.DateEvents;

public class IndexModel : PageModel
{
    private readonly CalendarDbContext _context;

    public IndexModel(CalendarDbContext context)
    {
        _context = context;
    }

    public IList<DateEvent> DateEvents { get; set; } = new List<DateEvent>();

    public async Task OnGetAsync()
    {
        DateEvents = await _context.DateEvents
            .OrderBy(e => e.Month)
            .ThenBy(e => e.Day)
            .ToListAsync();
    }
}
