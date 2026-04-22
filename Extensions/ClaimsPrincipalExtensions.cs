using System.Security.Claims;

namespace pattern_project.Extensions;

public static class ClaimsPrincipalExtensions
{
  public static int GetRequiredUserId(this ClaimsPrincipal principal)
  {
    var value = principal.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!int.TryParse(value, out var userId))
    {
      throw new UnauthorizedAccessException("Invalid user identity.");
    }

    return userId;
  }
}
