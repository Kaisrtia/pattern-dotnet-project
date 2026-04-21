using pattern_project.Models;

namespace pattern_project.Services;

public interface IAuthService
{
  Task<User?> AuthenticateAsync(string username, string password, CancellationToken cancellationToken);
}
