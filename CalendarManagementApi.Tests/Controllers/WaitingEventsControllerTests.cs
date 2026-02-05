using CalendarManagementApi.Controllers;
using CalendarManagementApi.DTOs;
using CalendarManagementApi.Models;
using CalendarManagementApi.Services;
using CalendarManagementApi.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CalendarManagementApi.Tests.Controllers;

[TestFixture]
public class WaitingEventsControllerTests : ControllerTestBase<WaitingEventsController>
{
    private WaitingEventsController CreateController()
    {
        var mockServiceLogger = new Mock<ILogger<WaitingEventService>>();
        var service = new WaitingEventService(Context, MockDateProvider.Object, mockServiceLogger.Object);
        return new WaitingEventsController(service, MockDateProvider.Object, MockLogger.Object);
    }

    #region GetAll Tests

    [Test]
    public async Task GetAll_ReturnsAllEvents()
    {
        var controller = CreateController();

        Context.WaitingEvents.AddRange(
            new WaitingEvent { Name = "Task 1", OccurrenceDate = new DateOnly(2026, 1, 15) },
            new WaitingEvent { Name = "Task 2", OccurrenceDate = new DateOnly(2026, 2, 20) }
        );
        await Context.SaveChangesAsync();

        var result = await controller.GetAll();

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result!;
        var events = (IEnumerable<WaitingEventDto>)okResult.Value!;
        Assert.That(events.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetAll_ReturnsEmptyListWhenNoEvents()
    {
        var controller = CreateController();

        var result = await controller.GetAll();

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result!;
        var events = (IEnumerable<WaitingEventDto>)okResult.Value!;
        Assert.That(events.Count(), Is.EqualTo(0));
    }

    #endregion

    #region GetById Tests

    [Test]
    public async Task GetById_ReturnsEventWhenExists()
    {
        var controller = CreateController();

        var waitingEvent = new WaitingEvent
        {
            Name = "Test Task",
            OccurrenceDate = new DateOnly(2026, 3, 15)
        };
        Context.WaitingEvents.Add(waitingEvent);
        await Context.SaveChangesAsync();

        var result = await controller.GetById(waitingEvent.Id);

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result!;
        var eventDto = (WaitingEventDto)okResult.Value!;
        Assert.That(eventDto.Name, Is.EqualTo("Test Task"));
    }

    [Test]
    public async Task GetById_Returns404WhenNotFound()
    {
        var controller = CreateController();

        var result = await controller.GetById(999);

        Assert.That(result.Result, Is.TypeOf<NotFoundResult>());
    }

    #endregion

    #region IsPastDue Tests

    [Test]
    public async Task GetAll_IsPastDueIsTrue_WhenOccurrenceDateIsBeforeToday()
    {
        var controller = CreateController();

        var pastDueEvent = new WaitingEvent
        {
            Name = "Past Due",
            OccurrenceDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-5))
        };
        Context.WaitingEvents.Add(pastDueEvent);
        await Context.SaveChangesAsync();

        var result = await controller.GetAll();

        var okResult = (OkObjectResult)result.Result!;
        var events = (IEnumerable<WaitingEventDto>)okResult.Value!;
        var eventDto = events.First();
        Assert.That(eventDto.IsPastDue, Is.True);
    }

    [Test]
    public async Task GetAll_IsPastDueIsTrue_WhenOccurrenceDateIsToday()
    {
        var controller = CreateController();

        var todayEvent = new WaitingEvent
        {
            Name = "Due Today",
            OccurrenceDate = DateOnly.FromDateTime(DateTime.Today)
        };
        Context.WaitingEvents.Add(todayEvent);
        await Context.SaveChangesAsync();

        var result = await controller.GetAll();

        var okResult = (OkObjectResult)result.Result!;
        var events = (IEnumerable<WaitingEventDto>)okResult.Value!;
        var eventDto = events.First();
        Assert.That(eventDto.IsPastDue, Is.True);
    }

    [Test]
    public async Task GetAll_IsPastDueIsFalse_WhenOccurrenceDateIsAfterToday()
    {
        var controller = CreateController();

        var futureEvent = new WaitingEvent
        {
            Name = "Future",
            OccurrenceDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7))
        };
        Context.WaitingEvents.Add(futureEvent);
        await Context.SaveChangesAsync();

        var result = await controller.GetAll();

        var okResult = (OkObjectResult)result.Result!;
        var events = (IEnumerable<WaitingEventDto>)okResult.Value!;
        var eventDto = events.First();
        Assert.That(eventDto.IsPastDue, Is.False);
    }

    [Test]
    public async Task GetById_IsPastDueIsCorrect()
    {
        var controller = CreateController();

        var pastDueEvent = new WaitingEvent
        {
            Name = "Past Due",
            OccurrenceDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1))
        };
        Context.WaitingEvents.Add(pastDueEvent);
        await Context.SaveChangesAsync();

        var result = await controller.GetById(pastDueEvent.Id);

        var okResult = (OkObjectResult)result.Result!;
        var eventDto = (WaitingEventDto)okResult.Value!;
        Assert.That(eventDto.IsPastDue, Is.True);
    }

    #endregion

    #region Create Tests

    [Test]
    public async Task Create_Returns201WithCreatedEvent()
    {
        var controller = CreateController();

        var dto = new CreateWaitingEventDto
        {
            Name = "New Task",
            OccurrenceDate = new DateOnly(2026, 6, 15),
            Layer = Layer.Red
        };

        var result = await controller.Create(dto);

        Assert.That(result.Result, Is.TypeOf<CreatedAtActionResult>());
        var createdResult = (CreatedAtActionResult)result.Result!;
        Assert.That(createdResult.StatusCode, Is.EqualTo(201));
        var eventDto = (WaitingEventDto)createdResult.Value!;
        Assert.That(eventDto.Name, Is.EqualTo("New Task"));
        Assert.That(eventDto.Id, Is.GreaterThan(0));
        Assert.That(eventDto.Layer, Is.EqualTo(Layer.Red));
    }

    [Test]
    public async Task Create_PersistsEventToDatabase()
    {
        var controller = CreateController();

        var dto = new CreateWaitingEventDto
        {
            Name = "Persisted Task",
            OccurrenceDate = new DateOnly(2026, 8, 20)
        };

        await controller.Create(dto);

        var savedEvent = Context.WaitingEvents.FirstOrDefault(e => e.Name == "Persisted Task");
        Assert.That(savedEvent, Is.Not.Null);
        Assert.That(savedEvent!.OccurrenceDate, Is.EqualTo(new DateOnly(2026, 8, 20)));
    }

    #endregion

    #region Update Tests

    [Test]
    public async Task Update_Returns204OnSuccess()
    {
        var controller = CreateController();

        var waitingEvent = new WaitingEvent
        {
            Name = "Original",
            OccurrenceDate = new DateOnly(2026, 1, 1)
        };
        Context.WaitingEvents.Add(waitingEvent);
        await Context.SaveChangesAsync();

        var updateDto = new UpdateWaitingEventDto
        {
            Name = "Updated",
            OccurrenceDate = new DateOnly(2026, 12, 31)
        };

        var result = await controller.Update(waitingEvent.Id, updateDto);

        Assert.That(result, Is.TypeOf<NoContentResult>());
    }

    [Test]
    public async Task Update_PersistsChangesToDatabase()
    {
        var controller = CreateController();

        var waitingEvent = new WaitingEvent
        {
            Name = "Original",
            OccurrenceDate = new DateOnly(2026, 1, 1)
        };
        Context.WaitingEvents.Add(waitingEvent);
        await Context.SaveChangesAsync();

        var updateDto = new UpdateWaitingEventDto
        {
            Name = "Updated",
            OccurrenceDate = new DateOnly(2026, 12, 31),
            Layer = Layer.Red
        };

        await controller.Update(waitingEvent.Id, updateDto);

        var updatedEvent = await Context.WaitingEvents.FindAsync(waitingEvent.Id);
        Assert.That(updatedEvent!.Name, Is.EqualTo("Updated"));
        Assert.That(updatedEvent.OccurrenceDate, Is.EqualTo(new DateOnly(2026, 12, 31)));
        Assert.That(updatedEvent.Layer, Is.EqualTo(Layer.Red));
    }

    [Test]
    public async Task Update_Returns404WhenNotFound()
    {
        var controller = CreateController();

        var updateDto = new UpdateWaitingEventDto
        {
            Name = "Updated",
            OccurrenceDate = new DateOnly(2026, 12, 31)
        };

        var result = await controller.Update(999, updateDto);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    #endregion

    #region Delete Tests

    [Test]
    public async Task Delete_Returns204OnSuccess()
    {
        var controller = CreateController();

        var waitingEvent = new WaitingEvent
        {
            Name = "To Delete",
            OccurrenceDate = new DateOnly(2026, 6, 15)
        };
        Context.WaitingEvents.Add(waitingEvent);
        await Context.SaveChangesAsync();

        var result = await controller.Delete(waitingEvent.Id);

        Assert.That(result, Is.TypeOf<NoContentResult>());
    }

    [Test]
    public async Task Delete_RemovesEventFromDatabase()
    {
        var controller = CreateController();

        var waitingEvent = new WaitingEvent
        {
            Name = "To Delete",
            OccurrenceDate = new DateOnly(2026, 6, 15)
        };
        Context.WaitingEvents.Add(waitingEvent);
        await Context.SaveChangesAsync();
        var eventId = waitingEvent.Id;

        await controller.Delete(eventId);

        var deletedEvent = await Context.WaitingEvents.FindAsync(eventId);
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

    #region PostponeWeek Tests

    [Test]
    public async Task PostponeWeek_Returns204OnSuccess()
    {
        var controller = CreateController();

        var waitingEvent = new WaitingEvent
        {
            Name = "Task",
            OccurrenceDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-5))
        };
        Context.WaitingEvents.Add(waitingEvent);
        await Context.SaveChangesAsync();

        var result = await controller.PostponeWeek(waitingEvent.Id);

        Assert.That(result, Is.TypeOf<NoContentResult>());
    }

    [Test]
    public async Task PostponeWeek_Adds7DaysFromToday()
    {
        var controller = CreateController();

        var waitingEvent = new WaitingEvent
        {
            Name = "Task",
            OccurrenceDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-5))
        };
        Context.WaitingEvents.Add(waitingEvent);
        await Context.SaveChangesAsync();

        await controller.PostponeWeek(waitingEvent.Id);

        var updatedEvent = await Context.WaitingEvents.FindAsync(waitingEvent.Id);
        var expectedDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7));
        Assert.That(updatedEvent!.OccurrenceDate, Is.EqualTo(expectedDate));
    }

    [Test]
    public async Task PostponeWeek_Returns404WhenNotFound()
    {
        var controller = CreateController();

        var result = await controller.PostponeWeek(999);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    #endregion

    #region PostponeMonth Tests

    [Test]
    public async Task PostponeMonth_Returns204OnSuccess()
    {
        var controller = CreateController();

        var waitingEvent = new WaitingEvent
        {
            Name = "Task",
            OccurrenceDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-5))
        };
        Context.WaitingEvents.Add(waitingEvent);
        await Context.SaveChangesAsync();

        var result = await controller.PostponeMonth(waitingEvent.Id);

        Assert.That(result, Is.TypeOf<NoContentResult>());
    }

    [Test]
    public async Task PostponeMonth_Adds1MonthFromToday()
    {
        var controller = CreateController();

        var waitingEvent = new WaitingEvent
        {
            Name = "Task",
            OccurrenceDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-5))
        };
        Context.WaitingEvents.Add(waitingEvent);
        await Context.SaveChangesAsync();

        await controller.PostponeMonth(waitingEvent.Id);

        var updatedEvent = await Context.WaitingEvents.FindAsync(waitingEvent.Id);
        var expectedDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(1));
        Assert.That(updatedEvent!.OccurrenceDate, Is.EqualTo(expectedDate));
    }

    [Test]
    public async Task PostponeMonth_Returns404WhenNotFound()
    {
        var controller = CreateController();

        var result = await controller.PostponeMonth(999);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    #endregion

    #region PostponeToDate Tests

    [Test]
    public async Task PostponeToDate_Returns204OnSuccess()
    {
        var controller = CreateController();

        var waitingEvent = new WaitingEvent
        {
            Name = "Task",
            OccurrenceDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-5))
        };
        Context.WaitingEvents.Add(waitingEvent);
        await Context.SaveChangesAsync();

        var dto = new PostponeDateDto { Date = new DateOnly(2026, 6, 15) };
        var result = await controller.PostponeToDate(waitingEvent.Id, dto);

        Assert.That(result, Is.TypeOf<NoContentResult>());
    }

    [Test]
    public async Task PostponeToDate_SetsCorrectDate()
    {
        var controller = CreateController();

        var waitingEvent = new WaitingEvent
        {
            Name = "Task",
            OccurrenceDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-5))
        };
        Context.WaitingEvents.Add(waitingEvent);
        await Context.SaveChangesAsync();

        var targetDate = new DateOnly(2026, 9, 20);
        var dto = new PostponeDateDto { Date = targetDate };
        await controller.PostponeToDate(waitingEvent.Id, dto);

        var updatedEvent = await Context.WaitingEvents.FindAsync(waitingEvent.Id);
        Assert.That(updatedEvent!.OccurrenceDate, Is.EqualTo(targetDate));
    }

    [Test]
    public async Task PostponeToDate_Returns404WhenNotFound()
    {
        var controller = CreateController();

        var dto = new PostponeDateDto { Date = new DateOnly(2026, 6, 15) };
        var result = await controller.PostponeToDate(999, dto);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    #endregion
}
