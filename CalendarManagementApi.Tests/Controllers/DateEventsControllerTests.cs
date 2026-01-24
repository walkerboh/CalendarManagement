using CalendarManagementApi.Controllers;
using CalendarManagementApi.DTOs;
using CalendarManagementApi.Models;
using CalendarManagementApi.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CalendarManagementApi.Tests.Controllers;

[TestFixture]
public class DateEventsControllerTests
{
    [Test]
    public async Task GetAll_ReturnsAllEvents()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<DateEventsController>>();
        var controller = new DateEventsController(context, mockLogger.Object);

        context.DateEvents.AddRange(
            new DateEvent { Name = "Birthday", Month = 7, Day = 4 },
            new DateEvent { Name = "Anniversary", Month = 12, Day = 25 }
        );
        await context.SaveChangesAsync();

        var result = await controller.GetAll();

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result!;
        var events = (IEnumerable<DateEventDto>)okResult.Value!;
        Assert.That(events.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetAll_ReturnsEmptyListWhenNoEvents()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<DateEventsController>>();
        var controller = new DateEventsController(context, mockLogger.Object);

        var result = await controller.GetAll();

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result!;
        var events = (IEnumerable<DateEventDto>)okResult.Value!;
        Assert.That(events.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task GetById_ReturnsEventWhenExists()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<DateEventsController>>();
        var controller = new DateEventsController(context, mockLogger.Object);

        var dateEvent = new DateEvent { Name = "Birthday", Month = 7, Day = 4 };
        context.DateEvents.Add(dateEvent);
        await context.SaveChangesAsync();

        var result = await controller.GetById(dateEvent.Id);

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result!;
        var eventDto = (DateEventDto)okResult.Value!;
        Assert.That(eventDto.Name, Is.EqualTo("Birthday"));
        Assert.That(eventDto.Month, Is.EqualTo(7));
        Assert.That(eventDto.Day, Is.EqualTo(4));
    }

    [Test]
    public async Task GetById_Returns404WhenNotFound()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<DateEventsController>>();
        var controller = new DateEventsController(context, mockLogger.Object);

        var result = await controller.GetById(999);

        Assert.That(result.Result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task Create_Returns201WithCreatedEvent()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<DateEventsController>>();
        var controller = new DateEventsController(context, mockLogger.Object);

        var dto = new CreateDateEventDto
        {
            Name = "New Birthday",
            Month = 5,
            Day = 15
        };

        var result = await controller.Create(dto);

        Assert.That(result.Result, Is.TypeOf<CreatedAtActionResult>());
        var createdResult = (CreatedAtActionResult)result.Result!;
        Assert.That(createdResult.StatusCode, Is.EqualTo(201));
        var eventDto = (DateEventDto)createdResult.Value!;
        Assert.That(eventDto.Name, Is.EqualTo("New Birthday"));
        Assert.That(eventDto.Id, Is.GreaterThan(0));
    }

    [Test]
    public async Task Create_PersistsEventToDatabase()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<DateEventsController>>();
        var controller = new DateEventsController(context, mockLogger.Object);

        var dto = new CreateDateEventDto
        {
            Name = "Persisted Event",
            Month = 3,
            Day = 20
        };

        await controller.Create(dto);

        var savedEvent = context.DateEvents.FirstOrDefault(e => e.Name == "Persisted Event");
        Assert.That(savedEvent, Is.Not.Null);
        Assert.That(savedEvent!.Month, Is.EqualTo(3));
        Assert.That(savedEvent.Day, Is.EqualTo(20));
    }

    [Test]
    public async Task Update_Returns204OnSuccess()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<DateEventsController>>();
        var controller = new DateEventsController(context, mockLogger.Object);

        var dateEvent = new DateEvent { Name = "Original", Month = 1, Day = 1 };
        context.DateEvents.Add(dateEvent);
        await context.SaveChangesAsync();

        var updateDto = new UpdateDateEventDto
        {
            Name = "Updated",
            Month = 12,
            Day = 31
        };

        var result = await controller.Update(dateEvent.Id, updateDto);

        Assert.That(result, Is.TypeOf<NoContentResult>());
    }

    [Test]
    public async Task Update_PersistsChangesToDatabase()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<DateEventsController>>();
        var controller = new DateEventsController(context, mockLogger.Object);

        var dateEvent = new DateEvent { Name = "Original", Month = 1, Day = 1 };
        context.DateEvents.Add(dateEvent);
        await context.SaveChangesAsync();

        var updateDto = new UpdateDateEventDto
        {
            Name = "Updated",
            Month = 12,
            Day = 31
        };

        await controller.Update(dateEvent.Id, updateDto);

        var updatedEvent = await context.DateEvents.FindAsync(dateEvent.Id);
        Assert.That(updatedEvent!.Name, Is.EqualTo("Updated"));
        Assert.That(updatedEvent.Month, Is.EqualTo(12));
        Assert.That(updatedEvent.Day, Is.EqualTo(31));
    }

    [Test]
    public async Task Update_Returns404WhenNotFound()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<DateEventsController>>();
        var controller = new DateEventsController(context, mockLogger.Object);

        var updateDto = new UpdateDateEventDto
        {
            Name = "Updated",
            Month = 12,
            Day = 31
        };

        var result = await controller.Update(999, updateDto);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task Delete_Returns204OnSuccess()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<DateEventsController>>();
        var controller = new DateEventsController(context, mockLogger.Object);

        var dateEvent = new DateEvent { Name = "To Delete", Month = 6, Day = 15 };
        context.DateEvents.Add(dateEvent);
        await context.SaveChangesAsync();

        var result = await controller.Delete(dateEvent.Id);

        Assert.That(result, Is.TypeOf<NoContentResult>());
    }

    [Test]
    public async Task Delete_RemovesEventFromDatabase()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<DateEventsController>>();
        var controller = new DateEventsController(context, mockLogger.Object);

        var dateEvent = new DateEvent { Name = "To Delete", Month = 6, Day = 15 };
        context.DateEvents.Add(dateEvent);
        await context.SaveChangesAsync();
        var eventId = dateEvent.Id;

        await controller.Delete(eventId);

        var deletedEvent = await context.DateEvents.FindAsync(eventId);
        Assert.That(deletedEvent, Is.Null);
    }

    [Test]
    public async Task Delete_Returns404WhenNotFound()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<DateEventsController>>();
        var controller = new DateEventsController(context, mockLogger.Object);

        var result = await controller.Delete(999);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }
}
