using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pattern_project.Models.Requests;
using pattern_project.Models.Responses;
using pattern_project.Services;

namespace pattern_project.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
  [HttpPost("login")]
  [AllowAnonymous]
  public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
  {
    var user = await authService.AuthenticateAsync(request.Username.Trim(), request.Password, cancellationToken);
    if (user is null)
    {
      return Unauthorized(new ProblemDetails
      {
        Title = "Unauthorized",
        Detail = "Invalid username or password.",
        Status = StatusCodes.Status401Unauthorized
      });
    }

    var claims = new List<Claim> {
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Name, user.Username),
        new(ClaimTypes.Role, user.Role.ToString())
    };

    var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

    return Ok(new AuthResponse
    {
      UserId = user.Id,
      Username = user.Username,
      Role = user.Role.ToString()
    });
  }

  [HttpPost("logout")]
  [Authorize]
  public async Task<IActionResult> Logout()
  {
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return NoContent();
  }
}
