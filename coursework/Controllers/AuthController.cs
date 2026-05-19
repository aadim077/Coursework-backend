using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleParts.Application.DTOs.Auth;
using VehicleParts.Application.Interfaces;
using VehicleParts.Domain.Enums;

namespace coursework.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _authService.RegisterAsync(request, cancellationToken);
        if (!response.IsSuccess)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpPost("customer/register")]
    [AllowAnonymous]
    public async Task<IActionResult> CustomerRegister([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        request.Role = UserRoles.Customer;

        var response = await _authService.RegisterAsync(request, cancellationToken);
        if (!response.IsSuccess)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _authService.LoginAsync(request, cancellationToken);
        if (!response.IsSuccess)
            return Unauthorized(response);

        return Ok(response);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _authService.RefreshTokenAsync(request, cancellationToken);
        if (!response.IsSuccess)
            return Unauthorized(response);

        return Ok(response);
    }

    [HttpPost("revoke")]
    [Authorize]
    public async Task<IActionResult> RevokeToken(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return BadRequest("Unable to identify user.");

        var result = await _authService.RevokeTokenAsync(userId, cancellationToken);
        if (!result)
            return BadRequest("Failed to revoke token.");

        return Ok("Token revoked successfully.");
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var fullName = User.FindFirst("FullName")?.Value;
        var roles = User.FindAll(System.Security.Claims.ClaimTypes.Role);

        return Ok(new
        {
            UserId = userId,
            Email = email,
            FullName = fullName,
            Roles = roles.Select(r => r.Value).ToList()
        });
    }
}
