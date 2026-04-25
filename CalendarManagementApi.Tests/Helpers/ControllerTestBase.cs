using CalendarManagementApi.Data;
using CalendarManagementApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Security.Claims;

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

    // [Authorize] is enforced by the MVC filter pipeline, not by controller methods directly.
    // Unit tests bypass the filter pipeline, so this helper is only needed when controller
    // actions read claims (e.g., User.Identity.Name).
    protected static void SetAuthenticatedUser(ControllerBase controller, string username = "testuser", string userId = "test-user-id")
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, username),
        ], "test"));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }
}
