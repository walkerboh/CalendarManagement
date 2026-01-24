using Microsoft.EntityFrameworkCore;
using CalendarManagementApi.Models;

namespace CalendarManagementApi.Data;

public class CalendarDbContext : DbContext
{
    public CalendarDbContext(DbContextOptions<CalendarDbContext> options) : base(options)
    {
    }

    public DbSet<DateEvent> DateEvents => Set<DateEvent>();
    public DbSet<WaitingEvent> WaitingEvents => Set<WaitingEvent>();
    public DbSet<RepeatingEvent> RepeatingEvents => Set<RepeatingEvent>();
}
