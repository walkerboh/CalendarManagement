using CalendarManagementApi.Data;
using CalendarManagementApi.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CalendarManagementApi.Tests.Helpers;

public abstract class ControllerTestBase<TController> where TController : class
{
    protected CalendarDbContext Context { get; private set; } = null!;
    protected Mock<ILogger<TController>> MockLogger { get; private set; } = null!;
    protected Mock<IDateProvider> MockDateProvider { get; private set; } = null!;

    [SetUp]
    public virtual void SetUp()
    {
        Context = TestDbContextFactory.Create();
        MockLogger = new Mock<ILogger<TController>>();
        MockDateProvider = new Mock<IDateProvider>();
        MockDateProvider.Setup(p => p.Today).Returns(DateOnly.FromDateTime(DateTime.Today));
    }

    [TearDown]
    public virtual void TearDown()
    {
        Context.Dispose();
    }
}
