using pattern_project.Models;
using pattern_project.Repositories;

namespace pattern_project.Services;

public class AuthService(IUserRepository userRepository) : IAuthService
{
  public async Task<User?> AuthenticateAsync(string username, string password, CancellationToken cancellationToken)
  {
    var user = await userRepository.GetByUsernameAsync(username, cancellationToken);

    if (user is null)
    {
      return null;
    }

    var providedHash = PasswordHasher.Hash(password);
    return providedHash == user.PasswordHash ? user : null;
  }
}
