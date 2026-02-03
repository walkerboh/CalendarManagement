using Microsoft.EntityFrameworkCore;
using CalendarManagementApi.Models;

namespace CalendarManagementApi.Data;

public class CalendarDbContext : DbContext
{
    public CalendarDbContext(DbContextOptions<CalendarDbContext> options) : base(options)
    {
    }

    public DbSet<MessageOfTheDay> MessagesOfTheDay => Set<MessageOfTheDay>();
    public DbSet<WaitingEvent> WaitingEvents => Set<WaitingEvent>();
    public DbSet<RepeatingEvent> RepeatingEvents => Set<RepeatingEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MessageOfTheDay>(entity =>
        {
            entity.ToTable("MessagesOfTheDay");
            entity.HasIndex(e => new { e.Month, e.Day }).IsUnique();
        });
    }
}
