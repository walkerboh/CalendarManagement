using CalendarManagement.Controllers;
using CalendarManagement.Data;
using CalendarManagement.DTOs;
using CalendarManagement.Models;
using CalendarManagement.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace CalendarManagement.Tests.Controllers;

[TestFixture]
public class AuthControllerTests
{
    private Mock<UserManager<IdentityUser>> _mockUserManager = null!;
    private Mock<SignInManager<IdentityUser>> _mockSignInManager = null!;
    private IConfiguration _configuration = null!;
    private CalendarDbContext _db = null!;
    private AuthController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        var store = new Mock<IUserStore<IdentityUser>>();
#pragma warning disable CS8625
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
                ["Auth:JwtAudience"] = "TestAudience",
                ["Auth:RefreshTokenExpiryDays"] = "7"
            })
            .Build();

        _db = TestDbContextFactory.Create();
        _controller = new AuthController(_mockUserManager.Object, _mockSignInManager.Object, _configuration, _db);
    }

    [TearDown]
    public void TearDown()
    {
        _db.Dispose();
    }

    [Test]
    public async Task Login_ValidCredentials_Returns200WithTokens()
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
        var response = ok.Value as AuthResponseDto;
        Assert.That(response, Is.Not.Null);
        Assert.That(response!.AccessToken, Is.Not.Null.And.Not.Empty);
        Assert.That(response.RefreshToken, Is.Not.Null.And.Not.Empty);
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

    [Test]
    public async Task Refresh_ValidToken_ReturnsNewTokenPair()
    {
        var userId = "user-1";
        var user = new IdentityUser { Id = userId, UserName = "admin" };
        _mockUserManager.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(user);

        var stored = new RefreshToken
        {
            Token = "valid-token",
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };
        _db.RefreshTokens.Add(stored);
        await _db.SaveChangesAsync();

        var result = await _controller.Refresh(new RefreshRequestDto("valid-token"));

        Assert.That(result, Is.TypeOf<OkObjectResult>());
        var ok = (OkObjectResult)result;
        var response = ok.Value as AuthResponseDto;
        Assert.That(response, Is.Not.Null);
        Assert.That(response!.AccessToken, Is.Not.Null.And.Not.Empty);
        Assert.That(response.RefreshToken, Is.Not.Null.And.Not.Empty);
        Assert.That(response.RefreshToken, Is.Not.EqualTo("valid-token"));

        Assert.That(stored.IsRevoked, Is.True);
    }

    [Test]
    public async Task Refresh_ExpiredToken_Returns401()
    {
        _db.RefreshTokens.Add(new RefreshToken
        {
            Token = "expired-token",
            UserId = "user-1",
            ExpiresAt = DateTime.UtcNow.AddDays(-1),
            CreatedAt = DateTime.UtcNow.AddDays(-8)
        });
        await _db.SaveChangesAsync();

        var result = await _controller.Refresh(new RefreshRequestDto("expired-token"));

        Assert.That(result, Is.TypeOf<UnauthorizedResult>());
    }

    [Test]
    public async Task Refresh_RevokedToken_Returns401()
    {
        _db.RefreshTokens.Add(new RefreshToken
        {
            Token = "revoked-token",
            UserId = "user-1",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = true
        });
        await _db.SaveChangesAsync();

        var result = await _controller.Refresh(new RefreshRequestDto("revoked-token"));

        Assert.That(result, Is.TypeOf<UnauthorizedResult>());
    }

    [Test]
    public async Task Refresh_UnknownToken_Returns401()
    {
        var result = await _controller.Refresh(new RefreshRequestDto("does-not-exist"));

        Assert.That(result, Is.TypeOf<UnauthorizedResult>());
    }

    [Test]
    public async Task Revoke_ValidToken_RevokesAndReturns204()
    {
        var stored = new RefreshToken
        {
            Token = "to-revoke",
            UserId = "user-1",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };
        _db.RefreshTokens.Add(stored);
        await _db.SaveChangesAsync();

        var result = await _controller.Revoke(new RefreshRequestDto("to-revoke"));

        Assert.That(result, Is.TypeOf<NoContentResult>());
        Assert.That(stored.IsRevoked, Is.True);
    }

    [Test]
    public async Task Revoke_UnknownToken_Returns204()
    {
        var result = await _controller.Revoke(new RefreshRequestDto("does-not-exist"));

        Assert.That(result, Is.TypeOf<NoContentResult>());
    }
}
