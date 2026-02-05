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
public class BirthdaysControllerTests
{
    [Test]
    public async Task GetAll_ReturnsAllBirthdays()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<BirthdaysController>>();
        var controller = new BirthdaysController(context, mockLogger.Object);

        context.Birthdays.AddRange(
            new Birthday { Name = "Alice", Month = 3, Day = 15 },
            new Birthday { Name = "Bob", Month = 7, Day = 4 }
        );
        await context.SaveChangesAsync();

        var result = await controller.GetAll();

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result!;
        var birthdays = (IEnumerable<BirthdayDto>)okResult.Value!;
        Assert.That(birthdays.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetAll_ReturnsEmptyListWhenNoBirthdays()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<BirthdaysController>>();
        var controller = new BirthdaysController(context, mockLogger.Object);

        var result = await controller.GetAll();

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result!;
        var birthdays = (IEnumerable<BirthdayDto>)okResult.Value!;
        Assert.That(birthdays.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task GetById_ReturnsBirthdayWhenExists()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<BirthdaysController>>();
        var controller = new BirthdaysController(context, mockLogger.Object);

        var birthday = new Birthday { Name = "Alice", Month = 3, Day = 15 };
        context.Birthdays.Add(birthday);
        await context.SaveChangesAsync();

        var result = await controller.GetById(birthday.Id);

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var okResult = (OkObjectResult)result.Result!;
        var dto = (BirthdayDto)okResult.Value!;
        Assert.That(dto.Name, Is.EqualTo("Alice"));
        Assert.That(dto.Month, Is.EqualTo(3));
        Assert.That(dto.Day, Is.EqualTo(15));
    }

    [Test]
    public async Task GetById_Returns404WhenNotFound()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<BirthdaysController>>();
        var controller = new BirthdaysController(context, mockLogger.Object);

        var result = await controller.GetById(999);

        Assert.That(result.Result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task Create_Returns201WithCreatedBirthday()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<BirthdaysController>>();
        var controller = new BirthdaysController(context, mockLogger.Object);

        var dto = new CreateBirthdayDto
        {
            Name = "Charlie",
            Month = 5,
            Day = 20
        };

        var result = await controller.Create(dto);

        Assert.That(result.Result, Is.TypeOf<CreatedAtActionResult>());
        var createdResult = (CreatedAtActionResult)result.Result!;
        Assert.That(createdResult.StatusCode, Is.EqualTo(201));
        var birthdayDto = (BirthdayDto)createdResult.Value!;
        Assert.That(birthdayDto.Name, Is.EqualTo("Charlie"));
        Assert.That(birthdayDto.Id, Is.GreaterThan(0));
    }

    [Test]
    public async Task Create_PersistsBirthdayToDatabase()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<BirthdaysController>>();
        var controller = new BirthdaysController(context, mockLogger.Object);

        var dto = new CreateBirthdayDto
        {
            Name = "Diana",
            Month = 11,
            Day = 8
        };

        await controller.Create(dto);

        var saved = context.Birthdays.FirstOrDefault(e => e.Name == "Diana");
        Assert.That(saved, Is.Not.Null);
        Assert.That(saved!.Month, Is.EqualTo(11));
        Assert.That(saved.Day, Is.EqualTo(8));
    }

    [Test]
    public async Task Create_AllowsDuplicateMonthDay()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<BirthdaysController>>();
        var controller = new BirthdaysController(context, mockLogger.Object);

        context.Birthdays.Add(new Birthday { Name = "Alice", Month = 5, Day = 15 });
        await context.SaveChangesAsync();

        var dto = new CreateBirthdayDto
        {
            Name = "Bob",
            Month = 5,
            Day = 15
        };

        var result = await controller.Create(dto);

        Assert.That(result.Result, Is.TypeOf<CreatedAtActionResult>());
        Assert.That(context.Birthdays.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task Update_Returns204OnSuccess()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<BirthdaysController>>();
        var controller = new BirthdaysController(context, mockLogger.Object);

        var birthday = new Birthday { Name = "Original", Month = 1, Day = 1 };
        context.Birthdays.Add(birthday);
        await context.SaveChangesAsync();

        var updateDto = new UpdateBirthdayDto
        {
            Name = "Updated",
            Month = 12,
            Day = 31
        };

        var result = await controller.Update(birthday.Id, updateDto);

        Assert.That(result, Is.TypeOf<NoContentResult>());
    }

    [Test]
    public async Task Update_PersistsChangesToDatabase()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<BirthdaysController>>();
        var controller = new BirthdaysController(context, mockLogger.Object);

        var birthday = new Birthday { Name = "Original", Month = 1, Day = 1 };
        context.Birthdays.Add(birthday);
        await context.SaveChangesAsync();

        var updateDto = new UpdateBirthdayDto
        {
            Name = "Updated",
            Month = 12,
            Day = 31
        };

        await controller.Update(birthday.Id, updateDto);

        var updated = await context.Birthdays.FindAsync(birthday.Id);
        Assert.That(updated!.Name, Is.EqualTo("Updated"));
        Assert.That(updated.Month, Is.EqualTo(12));
        Assert.That(updated.Day, Is.EqualTo(31));
    }

    [Test]
    public async Task Update_Returns404WhenNotFound()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<BirthdaysController>>();
        var controller = new BirthdaysController(context, mockLogger.Object);

        var updateDto = new UpdateBirthdayDto
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
        var mockLogger = new Mock<ILogger<BirthdaysController>>();
        var controller = new BirthdaysController(context, mockLogger.Object);

        var birthday = new Birthday { Name = "To Delete", Month = 6, Day = 15 };
        context.Birthdays.Add(birthday);
        await context.SaveChangesAsync();

        var result = await controller.Delete(birthday.Id);

        Assert.That(result, Is.TypeOf<NoContentResult>());
    }

    [Test]
    public async Task Delete_RemovesBirthdayFromDatabase()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<BirthdaysController>>();
        var controller = new BirthdaysController(context, mockLogger.Object);

        var birthday = new Birthday { Name = "To Delete", Month = 6, Day = 15 };
        context.Birthdays.Add(birthday);
        await context.SaveChangesAsync();
        var birthdayId = birthday.Id;

        await controller.Delete(birthdayId);

        var deleted = await context.Birthdays.FindAsync(birthdayId);
        Assert.That(deleted, Is.Null);
    }

    [Test]
    public async Task Delete_Returns404WhenNotFound()
    {
        var context = TestDbContextFactory.Create();
        var mockLogger = new Mock<ILogger<BirthdaysController>>();
        var controller = new BirthdaysController(context, mockLogger.Object);

        var result = await controller.Delete(999);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }
}
