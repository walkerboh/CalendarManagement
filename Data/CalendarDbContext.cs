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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DateEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<WaitingEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<RepeatingEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.WeeksOfMonth).HasMaxLength(20);
        });
    }
}
