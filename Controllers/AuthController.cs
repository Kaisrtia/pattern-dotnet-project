using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using pattern_project.Contracts.Requests;
using pattern_project.Services.Interfaces;

namespace pattern_project.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
  private readonly IAuthService _authService;

  public AuthController(IAuthService authService)
  {
    _authService = authService;
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
  {
    var loginResult = await _authService.ValidateLoginAsync(request.Username, request.Password, cancellationToken);

    var claims = new List<Claim>
    {
      new(ClaimTypes.NameIdentifier, loginResult.UserId.ToString()),
      new(ClaimTypes.Name, loginResult.Username),
      new(ClaimTypes.Role, loginResult.Role)
    };

    var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

    await HttpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        principal,
        new AuthenticationProperties
        {
          IsPersistent = true,
          ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
        });

    return Ok(new { message = "Login successful." });
  }

  [HttpPost("logout")]
  public async Task<IActionResult> Logout()
  {
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Ok(new { message = "Logout successful." });
  }
}
