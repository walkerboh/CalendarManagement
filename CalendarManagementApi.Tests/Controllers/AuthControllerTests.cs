using CalendarManagementApi.Controllers;
using CalendarManagementApi.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace CalendarManagementApi.Tests.Controllers;

[TestFixture]
public class AuthControllerTests
{
    private Mock<UserManager<IdentityUser>> _mockUserManager = null!;
    private Mock<SignInManager<IdentityUser>> _mockSignInManager = null!;
    private IConfiguration _configuration = null!;
    private AuthController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        var store = new Mock<IUserStore<IdentityUser>>();
#pragma warning disable CS8625 // Identity constructors have optional deps that can be null
        _mockUserManager = new Mock<UserManager<IdentityUser>>(
            store.Object, null, null, null, null, null, null, null, null);

        var contextAccessor = new Mock<IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();
        _mockSignInManager = new Mock<SignInManager<IdentityUser>>(
            _mockUserManager.Object, contextAccessor.Object, claimsFactory.Object,
            null, null, null, null);
#pragma warning restore CS8625

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Auth:JwtSecret"] = "test-secret-key-at-least-32-characters-long!!",
                ["Auth:JwtIssuer"] = "TestIssuer",
                ["Auth:JwtAudience"] = "TestAudience"
            })
            .Build();

        _controller = new AuthController(_mockUserManager.Object, _mockSignInManager.Object, _configuration);
    }

    [Test]
    public async Task Login_ValidCredentials_Returns200WithToken()
    {
        var user = new IdentityUser { Id = "1", UserName = "admin" };
        _mockUserManager.Setup(m => m.FindByNameAsync("admin")).ReturnsAsync(user);
        _mockSignInManager
            .Setup(m => m.CheckPasswordSignInAsync(user, "correctpassword", false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        var result = await _controller.Login(new LoginRequestDto
        {
            Username = "admin",
            Password = "correctpassword"
        });

        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var ok = (OkObjectResult)result;
        Assert.That(ok.Value, Is.Not.Null);

        var token = ok.Value!.GetType().GetProperty("token")?.GetValue(ok.Value) as string;
        Assert.That(token, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task Login_UserNotFound_Returns401()
    {
        _mockUserManager.Setup(m => m.FindByNameAsync("unknown")).ReturnsAsync((IdentityUser?)null);

        var result = await _controller.Login(new LoginRequestDto
        {
            Username = "unknown",
            Password = "any"
        });

        Assert.That(result, Is.TypeOf<UnauthorizedResult>());
    }

    [Test]
    public async Task Login_WrongPassword_Returns401()
    {
        var user = new IdentityUser { Id = "1", UserName = "admin" };
        _mockUserManager.Setup(m => m.FindByNameAsync("admin")).ReturnsAsync(user);
        _mockSignInManager
            .Setup(m => m.CheckPasswordSignInAsync(user, "wrongpassword", false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        var result = await _controller.Login(new LoginRequestDto
        {
            Username = "admin",
            Password = "wrongpassword"
        });

        Assert.That(result, Is.TypeOf<UnauthorizedResult>());
    }
}
