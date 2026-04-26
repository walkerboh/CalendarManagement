using CalendarManagement.Data;
using CalendarManagement.DTOs;
using CalendarManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CalendarManagement.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly CalendarDbContext _db;

    public AuthController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IConfiguration configuration,
        CalendarDbContext db)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _db = db;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
            return Unauthorized();

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
            return Unauthorized();

        var accessToken = GenerateJwtToken(user);
        var refreshToken = await CreateRefreshTokenAsync(user.Id);
        return Ok(new AuthResponseDto(accessToken, refreshToken));
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto request)
    {
        var stored = await _db.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == request.RefreshToken);

        if (stored == null || stored.IsRevoked || stored.ExpiresAt <= DateTime.UtcNow)
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(stored.UserId);
        if (user == null)
            return Unauthorized();

        stored.IsRevoked = true;
        var accessToken = GenerateJwtToken(user);
        var newRefreshToken = await CreateRefreshTokenAsync(user.Id);
        await _db.SaveChangesAsync();

        return Ok(new AuthResponseDto(accessToken, newRefreshToken));
    }

    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke([FromBody] RefreshRequestDto request)
    {
        var stored = await _db.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == request.RefreshToken);

        if (stored == null || stored.IsRevoked)
            return NoContent();

        stored.IsRevoked = true;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private string GenerateJwtToken(IdentityUser user)
    {
        var authConfig = _configuration.GetSection("Auth");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authConfig["JwtSecret"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!),
        };

        var expiryDays = authConfig.GetValue<int?>("AccessTokenExpiryDays") ?? 1;
        var token = new JwtSecurityToken(
            issuer: authConfig["JwtIssuer"],
            audience: authConfig["JwtAudience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(expiryDays),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<string> CreateRefreshTokenAsync(string userId)
    {
        var authConfig = _configuration.GetSection("Auth");
        var expiryDays = authConfig.GetValue<int?>("RefreshTokenExpiryDays") ?? 7;

        var tokenBytes = RandomNumberGenerator.GetBytes(64);
        var token = new RefreshToken
        {
            Token = Convert.ToBase64String(tokenBytes),
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
            CreatedAt = DateTime.UtcNow,
        };

        _db.RefreshTokens.Add(token);
        await _db.SaveChangesAsync();
        return token.Token;
    }
}
