using CalendarManagementApi.Data;
using Microsoft.EntityFrameworkCore;

namespace CalendarManagementApi.Tests.Helpers;

public static class TestDbContextFactory
{
    public static CalendarDbContext Create(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<CalendarDbContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options;

        var context = new CalendarDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}
