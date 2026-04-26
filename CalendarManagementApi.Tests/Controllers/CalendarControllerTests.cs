using CalendarManagement.Controllers;
using CalendarManagement.DTOs;
using CalendarManagement.Models;
using CalendarManagement.Services;
using CalendarManagement.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CalendarManagement.Tests.Controllers;

[TestFixture]
public class CalendarControllerTests : ControllerTestBase<CalendarController>
{
    private CalendarController CreateController()
    {
        var mockServiceLogger = new Mock<ILogger<CalendarService>>();
        var service = new CalendarService(Context, MockDateProvider.Object, mockServiceLogger.Object);
        return new CalendarController(service, MockLogger.Object);
    }

    [Test]
    public async Task GetEventsForDate_Returns200WithEvents()
    {
        var controller = CreateController();

        // Add a waiting event that is past due
        Context.WaitingEvents.Add(new WaitingEvent
        {
            Name = "Past Due Task",
            OccurrenceDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1))
        });
        // Add a repeating event for Monday
        Context.RepeatingEvents.Add(new RepeatingEvent
        {
            Name = "Monday Meeting",
            RepeatType = RepeatType.DayOfWeek,
            DayOfWeek = 1
        });
        await Context.SaveChangesAsync();

        var monday = new DateOnly(2026, 1, 26);
        var result = await controller.GetEventsForDate(monday);

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result!;
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task GetEventsForDate_ReturnsEmptyListWhenNoEvents()
    {
        var controller = CreateController();

        var result = await controller.GetEventsForDate(new DateOnly(2026, 1, 24));

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task GetMotdForDate_Returns200WithMessages()
    {
        var controller = CreateController();

        Context.MessagesOfTheDay.Add(new MessageOfTheDay
        {
            Message = "Birthday",
            Month = 7,
            Day = 4
        });
        await Context.SaveChangesAsync();

        var result = await controller.GetMotdForDate(new DateOnly(2026, 7, 4));

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result!;
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task GetMotdForDate_ReturnsEmptyListWhenNoMatches()
    {
        var controller = CreateController();

        Context.MessagesOfTheDay.Add(new MessageOfTheDay
        {
            Message = "Birthday",
            Month = 7,
            Day = 4
        });
        await Context.SaveChangesAsync();

        var result = await controller.GetMotdForDate(new DateOnly(2026, 8, 15));

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task GetBirthdaysForDate_Returns200WithBirthdays()
    {
        var controller = CreateController();

        Context.Birthdays.Add(new Birthday
        {
            Name = "Alice",
            Month = 7,
            Day = 4
        });
        await Context.SaveChangesAsync();

        var result = await controller.GetBirthdaysForDate(new DateOnly(2026, 7, 4));

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result!;
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task GetBirthdaysForDate_ReturnsEmptyListWhenNoMatches()
    {
        var controller = CreateController();

        Context.Birthdays.Add(new Birthday
        {
            Name = "Alice",
            Month = 7,
            Day = 4
        });
        await Context.SaveChangesAsync();

        // Aug 15 is outside the 14-day window from Jul 4
        var result = await controller.GetBirthdaysForDate(new DateOnly(2026, 8, 15));

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task GetBirthdaysForDate_ReturnsBirthdayResponseDtoWithCorrectFields()
    {
        var controller = CreateController();

        Context.Birthdays.Add(new Birthday
        {
            Name = "Alice",
            Month = 7,
            Day = 10
        });
        await Context.SaveChangesAsync();

        var result = await controller.GetBirthdaysForDate(new DateOnly(2026, 7, 4));

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result!;
        var birthdays = okResult.Value as List<BirthdayResponseDto>;
        Assert.That(birthdays, Is.Not.Null);
        Assert.That(birthdays, Has.Count.EqualTo(1));
        Assert.That(birthdays![0].Name, Is.EqualTo("Alice"));
        Assert.That(birthdays[0].Month, Is.EqualTo(7));
        Assert.That(birthdays[0].Day, Is.EqualTo(10));
        Assert.That(birthdays[0].DaysAway, Is.EqualTo(6));
    }

    [Test]
    public async Task GetBirthdaysForDate_ReturnsBirthdaysWithin14DayWindow()
    {
        var controller = CreateController();

        Context.Birthdays.Add(new Birthday { Name = "Day 0", Month = 7, Day = 4 });
        Context.Birthdays.Add(new Birthday { Name = "Day 14", Month = 7, Day = 18 });
        Context.Birthdays.Add(new Birthday { Name = "Day 15", Month = 7, Day = 19 }); // Outside window
        await Context.SaveChangesAsync();

        var result = await controller.GetBirthdaysForDate(new DateOnly(2026, 7, 4));

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result!;
        var birthdays = okResult.Value as List<BirthdayResponseDto>;
        Assert.That(birthdays, Has.Count.EqualTo(2));
        Assert.That(birthdays!.Select(b => b.Name), Is.EquivalentTo(new[] { "Day 0", "Day 14" }));
    }
}
