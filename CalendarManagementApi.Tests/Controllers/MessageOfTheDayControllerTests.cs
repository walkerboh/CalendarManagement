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
public class MessageOfTheDayControllerTests
{
    [Test]
    public async Task GetAll_ReturnsAllMessages()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<MessageOfTheDayController>>();
        var controller = new MessageOfTheDayController(context, mockLogger.Object);

        context.MessagesOfTheDay.AddRange(
            new MessageOfTheDay { Message = "Birthday", Month = 7, Day = 4 },
            new MessageOfTheDay { Message = "Anniversary", Month = 12, Day = 25 }
        );
        await context.SaveChangesAsync();

        var result = await controller.GetAll();

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result!;
        var messages = (IEnumerable<MessageOfTheDayDto>)okResult.Value!;
        Assert.That(messages.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetAll_ReturnsEmptyListWhenNoMessages()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<MessageOfTheDayController>>();
        var controller = new MessageOfTheDayController(context, mockLogger.Object);

        var result = await controller.GetAll();

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result!;
        var messages = (IEnumerable<MessageOfTheDayDto>)okResult.Value!;
        Assert.That(messages.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task GetById_ReturnsMessageWhenExists()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<MessageOfTheDayController>>();
        var controller = new MessageOfTheDayController(context, mockLogger.Object);

        var motd = new MessageOfTheDay { Message = "Birthday", Month = 7, Day = 4 };
        context.MessagesOfTheDay.Add(motd);
        await context.SaveChangesAsync();

        var result = await controller.GetById(motd.Id);

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result!;
        var dto = (MessageOfTheDayDto)okResult.Value!;
        Assert.That(dto.Message, Is.EqualTo("Birthday"));
        Assert.That(dto.Month, Is.EqualTo(7));
        Assert.That(dto.Day, Is.EqualTo(4));
    }

    [Test]
    public async Task GetById_Returns404WhenNotFound()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<MessageOfTheDayController>>();
        var controller = new MessageOfTheDayController(context, mockLogger.Object);

        var result = await controller.GetById(999);

        Assert.That(result.Result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task Create_Returns201WithCreatedMessage()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<MessageOfTheDayController>>();
        var controller = new MessageOfTheDayController(context, mockLogger.Object);

        var dto = new CreateMessageOfTheDayDto
        {
            Message = "New Birthday",
            Month = 5,
            Day = 15
        };

        var result = await controller.Create(dto);

        Assert.That(result.Result, Is.TypeOf<CreatedAtActionResult>());
        var createdResult = (CreatedAtActionResult)result.Result!;
        Assert.That(createdResult.StatusCode, Is.EqualTo(201));
        var motdDto = (MessageOfTheDayDto)createdResult.Value!;
        Assert.That(motdDto.Message, Is.EqualTo("New Birthday"));
        Assert.That(motdDto.Id, Is.GreaterThan(0));
    }

    [Test]
    public async Task Create_PersistsMessageToDatabase()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<MessageOfTheDayController>>();
        var controller = new MessageOfTheDayController(context, mockLogger.Object);

        var dto = new CreateMessageOfTheDayDto
        {
            Message = "Persisted Message",
            Month = 3,
            Day = 20
        };

        await controller.Create(dto);

        var saved = context.MessagesOfTheDay.FirstOrDefault(e => e.Message == "Persisted Message");
        Assert.That(saved, Is.Not.Null);
        Assert.That(saved!.Month, Is.EqualTo(3));
        Assert.That(saved.Day, Is.EqualTo(20));
    }

    [Test]
    public async Task Create_Returns409WhenDuplicateMonthDay()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<MessageOfTheDayController>>();
        var controller = new MessageOfTheDayController(context, mockLogger.Object);

        context.MessagesOfTheDay.Add(new MessageOfTheDay { Message = "Existing", Month = 5, Day = 15 });
        await context.SaveChangesAsync();

        var dto = new CreateMessageOfTheDayDto
        {
            Message = "Duplicate",
            Month = 5,
            Day = 15
        };

        var result = await controller.Create(dto);

        Assert.That(result.Result, Is.TypeOf<ConflictObjectResult>());
    }

    [Test]
    public async Task Update_Returns204OnSuccess()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<MessageOfTheDayController>>();
        var controller = new MessageOfTheDayController(context, mockLogger.Object);

        var motd = new MessageOfTheDay { Message = "Original", Month = 1, Day = 1 };
        context.MessagesOfTheDay.Add(motd);
        await context.SaveChangesAsync();

        var updateDto = new UpdateMessageOfTheDayDto
        {
            Message = "Updated",
            Month = 12,
            Day = 31
        };

        var result = await controller.Update(motd.Id, updateDto);

        Assert.That(result, Is.TypeOf<NoContentResult>());
    }

    [Test]
    public async Task Update_PersistsChangesToDatabase()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<MessageOfTheDayController>>();
        var controller = new MessageOfTheDayController(context, mockLogger.Object);

        var motd = new MessageOfTheDay { Message = "Original", Month = 1, Day = 1 };
        context.MessagesOfTheDay.Add(motd);
        await context.SaveChangesAsync();

        var updateDto = new UpdateMessageOfTheDayDto
        {
            Message = "Updated",
            Month = 12,
            Day = 31
        };

        await controller.Update(motd.Id, updateDto);

        var updated = await context.MessagesOfTheDay.FindAsync(motd.Id);
        Assert.That(updated!.Message, Is.EqualTo("Updated"));
        Assert.That(updated.Month, Is.EqualTo(12));
        Assert.That(updated.Day, Is.EqualTo(31));
    }

    [Test]
    public async Task Update_Returns404WhenNotFound()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<MessageOfTheDayController>>();
        var controller = new MessageOfTheDayController(context, mockLogger.Object);

        var updateDto = new UpdateMessageOfTheDayDto
        {
            Message = "Updated",
            Month = 12,
            Day = 31
        };

        var result = await controller.Update(999, updateDto);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task Update_Returns409WhenDuplicateMonthDay()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<MessageOfTheDayController>>();
        var controller = new MessageOfTheDayController(context, mockLogger.Object);

        context.MessagesOfTheDay.Add(new MessageOfTheDay { Message = "First", Month = 1, Day = 1 });
        context.MessagesOfTheDay.Add(new MessageOfTheDay { Message = "Second", Month = 6, Day = 15 });
        await context.SaveChangesAsync();

        var second = context.MessagesOfTheDay.First(e => e.Message == "Second");
        var updateDto = new UpdateMessageOfTheDayDto
        {
            Message = "Updated Second",
            Month = 1,
            Day = 1
        };

        var result = await controller.Update(second.Id, updateDto);

        Assert.That(result, Is.TypeOf<ConflictObjectResult>());
    }

    [Test]
    public async Task Delete_Returns204OnSuccess()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<MessageOfTheDayController>>();
        var controller = new MessageOfTheDayController(context, mockLogger.Object);

        var motd = new MessageOfTheDay { Message = "To Delete", Month = 6, Day = 15 };
        context.MessagesOfTheDay.Add(motd);
        await context.SaveChangesAsync();

        var result = await controller.Delete(motd.Id);

        Assert.That(result, Is.TypeOf<NoContentResult>());
    }

    [Test]
    public async Task Delete_RemovesMessageFromDatabase()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<MessageOfTheDayController>>();
        var controller = new MessageOfTheDayController(context, mockLogger.Object);

        var motd = new MessageOfTheDay { Message = "To Delete", Month = 6, Day = 15 };
        context.MessagesOfTheDay.Add(motd);
        await context.SaveChangesAsync();
        var motdId = motd.Id;

        await controller.Delete(motdId);

        var deleted = await context.MessagesOfTheDay.FindAsync(motdId);
        Assert.That(deleted, Is.Null);
    }

    [Test]
    public async Task Delete_Returns404WhenNotFound()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<MessageOfTheDayController>>();
        var controller = new MessageOfTheDayController(context, mockLogger.Object);

        var result = await controller.Delete(999);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }
}
