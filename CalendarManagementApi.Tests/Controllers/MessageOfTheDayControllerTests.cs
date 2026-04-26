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
public class MessageOfTheDayControllerTests : ControllerTestBase<MessageOfTheDayController>
{
    private MessageOfTheDayController CreateController()
    {
        var mockServiceLogger = new Mock<ILogger<MessageOfTheDayService>>();
        var service = new MessageOfTheDayService(Context, mockServiceLogger.Object);
        return new MessageOfTheDayController(service, MockLogger.Object);
    }

    [Test]
    public async Task GetAll_ReturnsAllMessages()
    {
        var controller = CreateController();

        Context.MessagesOfTheDay.AddRange(
            new MessageOfTheDay { Message = "Birthday", Month = 7, Day = 4 },
            new MessageOfTheDay { Message = "Anniversary", Month = 12, Day = 25 }
        );
        await Context.SaveChangesAsync();

        var result = await controller.GetAll();

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result!;
        var messages = (IEnumerable<MessageOfTheDayDto>)okResult.Value!;
        Assert.That(messages.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetAll_ReturnsEmptyListWhenNoMessages()
    {
        var controller = CreateController();

        var result = await controller.GetAll();

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result!;
        var messages = (IEnumerable<MessageOfTheDayDto>)okResult.Value!;
        Assert.That(messages.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task GetById_ReturnsMessageWhenExists()
    {
        var controller = CreateController();

        var motd = new MessageOfTheDay { Message = "Birthday", Month = 7, Day = 4, Layer = Layer.Red };
        Context.MessagesOfTheDay.Add(motd);
        await Context.SaveChangesAsync();

        var result = await controller.GetById(motd.Id);

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result!;
        var dto = (MessageOfTheDayDto)okResult.Value!;
        Assert.That(dto.Message, Is.EqualTo("Birthday"));
        Assert.That(dto.Month, Is.EqualTo(7));
        Assert.That(dto.Day, Is.EqualTo(4));
        Assert.That(dto.Layer, Is.EqualTo(Layer.Red));
    }

    [Test]
    public async Task GetById_Returns404WhenNotFound()
    {
        var controller = CreateController();

        var result = await controller.GetById(999);

        Assert.That(result.Result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task Create_Returns201WithCreatedMessage()
    {
        var controller = CreateController();

        var dto = new CreateMessageOfTheDayDto
        {
            Message = "New Birthday",
            Month = 5,
            Day = 15,
            Layer = Layer.Red
        };

        var result = await controller.Create(dto);

        Assert.That(result.Result, Is.TypeOf<CreatedAtActionResult>());
        var createdResult = (CreatedAtActionResult)result.Result!;
        Assert.That(createdResult.StatusCode, Is.EqualTo(201));
        var motdDto = (MessageOfTheDayDto)createdResult.Value!;
        Assert.That(motdDto.Message, Is.EqualTo("New Birthday"));
        Assert.That(motdDto.Id, Is.GreaterThan(0));
        Assert.That(motdDto.Layer, Is.EqualTo(Layer.Red));
    }

    [Test]
    public async Task Create_PersistsMessageToDatabase()
    {
        var controller = CreateController();

        var dto = new CreateMessageOfTheDayDto
        {
            Message = "Persisted Message",
            Month = 3,
            Day = 20
        };

        await controller.Create(dto);

        var saved = Context.MessagesOfTheDay.FirstOrDefault(e => e.Message == "Persisted Message");
        Assert.That(saved, Is.Not.Null);
        Assert.That(saved!.Month, Is.EqualTo(3));
        Assert.That(saved.Day, Is.EqualTo(20));
    }

    [Test]
    public async Task Create_Returns409WhenDuplicateMonthDay()
    {
        var controller = CreateController();

        Context.MessagesOfTheDay.Add(new MessageOfTheDay { Message = "Existing", Month = 5, Day = 15 });
        await Context.SaveChangesAsync();

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
        var controller = CreateController();

        var motd = new MessageOfTheDay { Message = "Original", Month = 1, Day = 1 };
        Context.MessagesOfTheDay.Add(motd);
        await Context.SaveChangesAsync();

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
        var controller = CreateController();

        var motd = new MessageOfTheDay { Message = "Original", Month = 1, Day = 1 };
        Context.MessagesOfTheDay.Add(motd);
        await Context.SaveChangesAsync();

        var updateDto = new UpdateMessageOfTheDayDto
        {
            Message = "Updated",
            Month = 12,
            Day = 31,
            Layer = Layer.Red
        };

        await controller.Update(motd.Id, updateDto);

        var updated = await Context.MessagesOfTheDay.FindAsync(motd.Id);
        Assert.That(updated!.Message, Is.EqualTo("Updated"));
        Assert.That(updated.Month, Is.EqualTo(12));
        Assert.That(updated.Day, Is.EqualTo(31));
        Assert.That(updated.Layer, Is.EqualTo(Layer.Red));
    }

    [Test]
    public async Task Update_Returns404WhenNotFound()
    {
        var controller = CreateController();

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
        var controller = CreateController();

        Context.MessagesOfTheDay.Add(new MessageOfTheDay { Message = "First", Month = 1, Day = 1 });
        Context.MessagesOfTheDay.Add(new MessageOfTheDay { Message = "Second", Month = 6, Day = 15 });
        await Context.SaveChangesAsync();

        var second = Context.MessagesOfTheDay.First(e => e.Message == "Second");
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
        var controller = CreateController();

        var motd = new MessageOfTheDay { Message = "To Delete", Month = 6, Day = 15 };
        Context.MessagesOfTheDay.Add(motd);
        await Context.SaveChangesAsync();

        var result = await controller.Delete(motd.Id);

        Assert.That(result, Is.TypeOf<NoContentResult>());
    }

    [Test]
    public async Task Delete_RemovesMessageFromDatabase()
    {
        var controller = CreateController();

        var motd = new MessageOfTheDay { Message = "To Delete", Month = 6, Day = 15 };
        Context.MessagesOfTheDay.Add(motd);
        await Context.SaveChangesAsync();
        var motdId = motd.Id;

        await controller.Delete(motdId);

        var deleted = await Context.MessagesOfTheDay.FindAsync(motdId);
        Assert.That(deleted, Is.Null);
    }

    [Test]
    public async Task Delete_Returns404WhenNotFound()
    {
        var controller = CreateController();

        var result = await controller.Delete(999);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }
}
