using System.Security.Claims;
using pattern_project.Services.Exceptions;

namespace pattern_project.Services;

public class UserContextService(IHttpContextAccessor httpContextAccessor) : IUserContextService
{
  public long GetRequiredUserId()
  {
    var claimValue = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

    if (!long.TryParse(claimValue, out var userId))
    {
      throw new ForbiddenException("User identity is missing or invalid.");
    }

    return userId;
  }

  public bool IsAdmin()
  {
    return httpContextAccessor.HttpContext?.User.IsInRole("Admin") == true;
  }
}
