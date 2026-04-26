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
public class RepeatingEventsControllerTests : ControllerTestBase<RepeatingEventsController>
{
    private RepeatingEventsController CreateController()
    {
        var mockServiceLogger = new Mock<ILogger<RepeatingEventService>>();
        var service = new RepeatingEventService(Context, mockServiceLogger.Object);
        return new RepeatingEventsController(service, MockLogger.Object);
    }

    #region GetAll Tests

    [Test]
    public async Task GetAll_ReturnsAllEvents()
    {
        var controller = CreateController();

        Context.RepeatingEvents.AddRange(
            new RepeatingEvent { Name = "Weekly", RepeatType = RepeatType.DayOfWeek, DayOfWeek = 1 },
            new RepeatingEvent { Name = "Monthly", RepeatType = RepeatType.DayOfWeekOfMonth, DayOfWeek = 2, WeeksOfMonth = "1,3" }
        );
        await Context.SaveChangesAsync();

        var result = await controller.GetAll();

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result!;
        var events = (IEnumerable<RepeatingEventDto>)okResult.Value!;
        Assert.That(events.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetAll_ReturnsEmptyListWhenNoEvents()
    {
        var controller = CreateController();

        var result = await controller.GetAll();

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result!;
        var events = (IEnumerable<RepeatingEventDto>)okResult.Value!;
        Assert.That(events.Count(), Is.EqualTo(0));
    }

    #endregion

    #region GetById Tests

    [Test]
    public async Task GetById_ReturnsEventWhenExists()
    {
        var controller = CreateController();

        var repeatingEvent = new RepeatingEvent
        {
            Name = "Weekly Meeting",
            RepeatType = RepeatType.DayOfWeek,
            DayOfWeek = 1
        };
        Context.RepeatingEvents.Add(repeatingEvent);
        await Context.SaveChangesAsync();

        var result = await controller.GetById(repeatingEvent.Id);

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result!;
        var eventDto = (RepeatingEventDto)okResult.Value!;
        Assert.That(eventDto.Name, Is.EqualTo("Weekly Meeting"));
        Assert.That(eventDto.RepeatType, Is.EqualTo(RepeatType.DayOfWeek));
        Assert.That(eventDto.DayOfWeek, Is.EqualTo(1));
    }

    [Test]
    public async Task GetById_Returns404WhenNotFound()
    {
        var controller = CreateController();

        var result = await controller.GetById(999);

        Assert.That(result.Result, Is.TypeOf<NotFoundResult>());
    }

    #endregion

    #region Create Tests - DayOfWeek Type

    [Test]
    public async Task Create_DayOfWeekType_Returns201WithCreatedEvent()
    {
        var controller = CreateController();

        var dto = new CreateRepeatingEventDto
        {
            Name = "Monday Meeting",
            RepeatType = RepeatType.DayOfWeek,
            DayOfWeek = 1,
            Layer = Layer.Red
        };

        var result = await controller.Create(dto);

        Assert.That(result.Result, Is.TypeOf<CreatedAtActionResult>());
        var createdResult = (CreatedAtActionResult)result.Result!;
        Assert.That(createdResult.StatusCode, Is.EqualTo(201));
        var eventDto = (RepeatingEventDto)createdResult.Value!;
        Assert.That(eventDto.Name, Is.EqualTo("Monday Meeting"));
        Assert.That(eventDto.RepeatType, Is.EqualTo(RepeatType.DayOfWeek));
        Assert.That(eventDto.Layer, Is.EqualTo(Layer.Red));
    }

    #endregion

    #region Create Tests - DayOfWeekOfMonth Type

    [Test]
    public async Task Create_DayOfWeekOfMonthType_Returns201WithCreatedEvent()
    {
        var controller = CreateController();

        var dto = new CreateRepeatingEventDto
        {
            Name = "First Monday of Month",
            RepeatType = RepeatType.DayOfWeekOfMonth,
            DayOfWeek = 1,
            WeeksOfMonth = "1"
        };

        var result = await controller.Create(dto);

        Assert.That(result.Result, Is.TypeOf<CreatedAtActionResult>());
        var createdResult = (CreatedAtActionResult)result.Result!;
        var eventDto = (RepeatingEventDto)createdResult.Value!;
        Assert.That(eventDto.RepeatType, Is.EqualTo(RepeatType.DayOfWeekOfMonth));
        Assert.That(eventDto.WeeksOfMonth, Is.EqualTo("1"));
    }

    [Test]
    public async Task Create_DayOfWeekOfMonthType_WithMultipleWeeks_PersistsCorrectly()
    {
        var controller = CreateController();

        var dto = new CreateRepeatingEventDto
        {
            Name = "Bi-weekly Friday",
            RepeatType = RepeatType.DayOfWeekOfMonth,
            DayOfWeek = 5, // Friday
            WeeksOfMonth = "1,3"
        };

        await controller.Create(dto);

        var savedEvent = Context.RepeatingEvents.FirstOrDefault(e => e.Name == "Bi-weekly Friday");
        Assert.That(savedEvent, Is.Not.Null);
        Assert.That(savedEvent!.WeeksOfMonth, Is.EqualTo("1,3"));
    }

    #endregion

    #region Create Tests - Interval Type

    [Test]
    public async Task Create_IntervalType_Returns201WithCreatedEvent()
    {
        var controller = CreateController();

        var dto = new CreateRepeatingEventDto
        {
            Name = "Every 2 Weeks",
            RepeatType = RepeatType.Interval,
            StartDate = new DateOnly(2026, 1, 1),
            IntervalDays = 14
        };

        var result = await controller.Create(dto);

        Assert.That(result.Result, Is.TypeOf<CreatedAtActionResult>());
        var createdResult = (CreatedAtActionResult)result.Result!;
        var eventDto = (RepeatingEventDto)createdResult.Value!;
        Assert.That(eventDto.RepeatType, Is.EqualTo(RepeatType.Interval));
        Assert.That(eventDto.IntervalDays, Is.EqualTo(14));
        Assert.That(eventDto.StartDate, Is.EqualTo(new DateOnly(2026, 1, 1)));
    }

    #endregion

    #region Create Tests - Date Type

    [Test]
    public async Task Create_DateType_Returns201WithCreatedEvent()
    {
        var controller = CreateController();

        var dto = new CreateRepeatingEventDto
        {
            Name = "Independence Day",
            RepeatType = RepeatType.Date,
            Month = 7,
            Day = 4
        };

        var result = await controller.Create(dto);

        Assert.That(result.Result, Is.TypeOf<CreatedAtActionResult>());
        var createdResult = (CreatedAtActionResult)result.Result!;
        var eventDto = (RepeatingEventDto)createdResult.Value!;
        Assert.That(eventDto.RepeatType, Is.EqualTo(RepeatType.Date));
        Assert.That(eventDto.Month, Is.EqualTo(7));
        Assert.That(eventDto.Day, Is.EqualTo(4));
    }

    [Test]
    public async Task Create_DateType_MapsMonthAndDay()
    {
        var controller = CreateController();

        var dto = new CreateRepeatingEventDto
        {
            Name = "Christmas",
            RepeatType = RepeatType.Date,
            Month = 12,
            Day = 25
        };

        await controller.Create(dto);

        var savedEvent = Context.RepeatingEvents.FirstOrDefault(e => e.Name == "Christmas");
        Assert.That(savedEvent, Is.Not.Null);
        Assert.That(savedEvent!.Month, Is.EqualTo(12));
        Assert.That(savedEvent.Day, Is.EqualTo(25));
    }

    #endregion

    #region Update Tests

    [Test]
    public async Task Update_Returns204OnSuccess()
    {
        var controller = CreateController();

        var repeatingEvent = new RepeatingEvent
        {
            Name = "Original",
            RepeatType = RepeatType.DayOfWeek,
            DayOfWeek = 1
        };
        Context.RepeatingEvents.Add(repeatingEvent);
        await Context.SaveChangesAsync();

        var updateDto = new UpdateRepeatingEventDto
        {
            Name = "Updated",
            RepeatType = RepeatType.DayOfWeek,
            DayOfWeek = 5
        };

        var result = await controller.Update(repeatingEvent.Id, updateDto);

        Assert.That(result, Is.TypeOf<NoContentResult>());
    }

    [Test]
    public async Task Update_PersistsChangesToDatabase()
    {
        var controller = CreateController();

        var repeatingEvent = new RepeatingEvent
        {
            Name = "Original",
            RepeatType = RepeatType.DayOfWeek,
            DayOfWeek = 1
        };
        Context.RepeatingEvents.Add(repeatingEvent);
        await Context.SaveChangesAsync();

        var updateDto = new UpdateRepeatingEventDto
        {
            Name = "Updated Meeting",
            RepeatType = RepeatType.Interval,
            StartDate = new DateOnly(2026, 5, 1),
            IntervalDays = 7,
            Layer = Layer.Red
        };

        await controller.Update(repeatingEvent.Id, updateDto);

        var updatedEvent = await Context.RepeatingEvents.FindAsync(repeatingEvent.Id);
        Assert.That(updatedEvent!.Name, Is.EqualTo("Updated Meeting"));
        Assert.That(updatedEvent.RepeatType, Is.EqualTo(RepeatType.Interval));
        Assert.That(updatedEvent.IntervalDays, Is.EqualTo(7));
        Assert.That(updatedEvent.Layer, Is.EqualTo(Layer.Red));
    }

    [Test]
    public async Task Update_Returns404WhenNotFound()
    {
        var controller = CreateController();

        var updateDto = new UpdateRepeatingEventDto
        {
            Name = "Updated",
            RepeatType = RepeatType.DayOfWeek,
            DayOfWeek = 5
        };

        var result = await controller.Update(999, updateDto);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task Update_CanChangeRepeatType()
    {
        var controller = CreateController();

        var repeatingEvent = new RepeatingEvent
        {
            Name = "Event",
            RepeatType = RepeatType.DayOfWeek,
            DayOfWeek = 1
        };
        Context.RepeatingEvents.Add(repeatingEvent);
        await Context.SaveChangesAsync();

        var updateDto = new UpdateRepeatingEventDto
        {
            Name = "Event",
            RepeatType = RepeatType.Date,
            Month = 7,
            Day = 4
        };

        await controller.Update(repeatingEvent.Id, updateDto);

        var updatedEvent = await Context.RepeatingEvents.FindAsync(repeatingEvent.Id);
        Assert.That(updatedEvent!.RepeatType, Is.EqualTo(RepeatType.Date));
        Assert.That(updatedEvent.Month, Is.EqualTo(7));
        Assert.That(updatedEvent.Day, Is.EqualTo(4));
    }

    #endregion

    #region Delete Tests

    [Test]
    public async Task Delete_Returns204OnSuccess()
    {
        var controller = CreateController();

        var repeatingEvent = new RepeatingEvent
        {
            Name = "To Delete",
            RepeatType = RepeatType.DayOfWeek,
            DayOfWeek = 1
        };
        Context.RepeatingEvents.Add(repeatingEvent);
        await Context.SaveChangesAsync();

        var result = await controller.Delete(repeatingEvent.Id);

        Assert.That(result, Is.TypeOf<NoContentResult>());
    }

    [Test]
    public async Task Delete_RemovesEventFromDatabase()
    {
        var controller = CreateController();

        var repeatingEvent = new RepeatingEvent
        {
            Name = "To Delete",
            RepeatType = RepeatType.DayOfWeek,
            DayOfWeek = 1
        };
        Context.RepeatingEvents.Add(repeatingEvent);
        await Context.SaveChangesAsync();
        var eventId = repeatingEvent.Id;

        await controller.Delete(eventId);

        var deletedEvent = await Context.RepeatingEvents.FindAsync(eventId);
        Assert.That(deletedEvent, Is.Null);
    }

    [Test]
    public async Task Delete_Returns404WhenNotFound()
    {
        var controller = CreateController();

        var result = await controller.Delete(999);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    #endregion

    #region All RepeatType Values Tests

    [Test]
    public async Task GetById_ReturnsCorrectDataForAllRepeatTypes()
    {
        var controller = CreateController();

        var dayOfWeekEvent = new RepeatingEvent
        {
            Name = "DayOfWeek Event",
            RepeatType = RepeatType.DayOfWeek,
            DayOfWeek = 1
        };
        var dayOfWeekOfMonthEvent = new RepeatingEvent
        {
            Name = "DayOfWeekOfMonth Event",
            RepeatType = RepeatType.DayOfWeekOfMonth,
            DayOfWeek = 2,
            WeeksOfMonth = "1,3"
        };
        var intervalEvent = new RepeatingEvent
        {
            Name = "Interval Event",
            RepeatType = RepeatType.Interval,
            StartDate = new DateOnly(2026, 1, 1),
            IntervalDays = 7
        };
        var dateEvent = new RepeatingEvent
        {
            Name = "Date Event",
            RepeatType = RepeatType.Date,
            Month = 7,
            Day = 4
        };

        Context.RepeatingEvents.AddRange(dayOfWeekEvent, dayOfWeekOfMonthEvent, intervalEvent, dateEvent);
        await Context.SaveChangesAsync();

        // Test DayOfWeek
        var result1 = await controller.GetById(dayOfWeekEvent.Id);
        var dto1 = (RepeatingEventDto)((OkObjectResult)result1.Result!).Value!;
        Assert.That(dto1.RepeatType, Is.EqualTo(RepeatType.DayOfWeek));
        Assert.That(dto1.DayOfWeek, Is.EqualTo(1));

        // Test DayOfWeekOfMonth
        var result2 = await controller.GetById(dayOfWeekOfMonthEvent.Id);
        var dto2 = (RepeatingEventDto)((OkObjectResult)result2.Result!).Value!;
        Assert.That(dto2.RepeatType, Is.EqualTo(RepeatType.DayOfWeekOfMonth));
        Assert.That(dto2.WeeksOfMonth, Is.EqualTo("1,3"));

        // Test Interval
        var result3 = await controller.GetById(intervalEvent.Id);
        var dto3 = (RepeatingEventDto)((OkObjectResult)result3.Result!).Value!;
        Assert.That(dto3.RepeatType, Is.EqualTo(RepeatType.Interval));
        Assert.That(dto3.IntervalDays, Is.EqualTo(7));

        // Test Date
        var result4 = await controller.GetById(dateEvent.Id);
        var dto4 = (RepeatingEventDto)((OkObjectResult)result4.Result!).Value!;
        Assert.That(dto4.RepeatType, Is.EqualTo(RepeatType.Date));
        Assert.That(dto4.Month, Is.EqualTo(7));
        Assert.That(dto4.Day, Is.EqualTo(4));
    }

    #endregion
}
