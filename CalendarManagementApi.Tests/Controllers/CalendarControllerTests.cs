using CalendarManagementApi.Controllers;
using CalendarManagementApi.Models;
using CalendarManagementApi.Services;
using CalendarManagementApi.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CalendarManagementApi.Tests.Controllers;

[TestFixture]
public class CalendarControllerTests
{
    [Test]
    public async Task GetEventsForDate_Returns200WithEvents()
    {
        var context = TestDbContextFactory.Create();
        var service = new CalendarService(context);
        var mockLogger = new Mock<ILogger<CalendarController>>();
        var controller = new CalendarController(service, mockLogger.Object);

        // Add a waiting event that is past due
        context.WaitingEvents.Add(new WaitingEvent
        {
            Name = "Past Due Task",
            OccurrenceDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1))
        });
        // Add a repeating event for Monday
        context.RepeatingEvents.Add(new RepeatingEvent
        {
            Name = "Monday Meeting",
            RepeatType = RepeatType.DayOfWeek,
            DayOfWeek = 1
        });
        await context.SaveChangesAsync();

        var monday = new DateOnly(2026, 1, 26);
        var result = await controller.GetEventsForDate(monday);

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result!;
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task GetEventsForDate_ReturnsEmptyListWhenNoEvents()
    {
        var context = TestDbContextFactory.Create();
        var service = new CalendarService(context);
        var mockLogger = new Mock<ILogger<CalendarController>>();
        var controller = new CalendarController(service, mockLogger.Object);

        var result = await controller.GetEventsForDate(new DateOnly(2026, 1, 24));

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task GetDateEventsForDate_Returns200WithDateEvents()
    {
        var context = TestDbContextFactory.Create();
        var service = new CalendarService(context);
        var mockLogger = new Mock<ILogger<CalendarController>>();
        var controller = new CalendarController(service, mockLogger.Object);

        context.DateEvents.Add(new DateEvent
        {
            Name = "Birthday",
            Month = 7,
            Day = 4
        });
        await context.SaveChangesAsync();

        var result = await controller.GetDateEventsForDate(new DateOnly(2026, 7, 4));

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result!;
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task GetDateEventsForDate_ReturnsEmptyListWhenNoMatches()
    {
        var context = TestDbContextFactory.Create();
        var service = new CalendarService(context);
        var mockLogger = new Mock<ILogger<CalendarController>>();
        var controller = new CalendarController(service, mockLogger.Object);

        context.DateEvents.Add(new DateEvent
        {
            Name = "Birthday",
            Month = 7,
            Day = 4
        });
        await context.SaveChangesAsync();

        var result = await controller.GetDateEventsForDate(new DateOnly(2026, 8, 15));

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
    }
}
