using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CalendarManagementApi.Data;
using CalendarManagementApi.Models;

namespace CalendarManagementApi.Pages.RepeatingEvents;

public class IndexModel : PageModel
{
    private readonly CalendarDbContext _context;

    public IndexModel(CalendarDbContext context)
    {
        _context = context;
    }

    public IList<RepeatingEvent> RepeatingEvents { get; set; } = new List<RepeatingEvent>();

    public async Task OnGetAsync()
    {
        RepeatingEvents = await _context.RepeatingEvents
            .OrderBy(e => e.Name)
            .ToListAsync();
    }
}
