using CalendarManagementApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CalendarManagementApi.Data;

public class CalendarDbContext : IdentityDbContext<IdentityUser>
{
    public CalendarDbContext(DbContextOptions<CalendarDbContext> options) : base(options)
    {
    }

    public DbSet<MessageOfTheDay> MessagesOfTheDay => Set<MessageOfTheDay>();
    public DbSet<WaitingEvent> WaitingEvents => Set<WaitingEvent>();
    public DbSet<RepeatingEvent> RepeatingEvents => Set<RepeatingEvent>();
    public DbSet<Birthday> Birthdays => Set<Birthday>();

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
