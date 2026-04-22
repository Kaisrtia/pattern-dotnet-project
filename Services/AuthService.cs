using pattern_project.Domain.Exceptions;
using pattern_project.Repositories.Interfaces;
using pattern_project.Services.Interfaces;

namespace pattern_project.Services;

public sealed class AuthService : IAuthService
{
  private readonly IUserRepository _userRepository;

  public AuthService(IUserRepository userRepository)
  {
    _userRepository = userRepository;
  }

  public async Task<(int UserId, string Username, string Role)> ValidateLoginAsync(string username, string password, CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
    {
      throw new DomainValidationException("Username and password are required.");
    }

    var user = await _userRepository.FindByUsernameAsync(username.Trim(), cancellationToken);
    if (user is null || user.PasswordHash != password)
    {
      throw new UnauthorizedAccessException("Invalid credentials.");
    }

    return (user.Id, user.Username, user.Role.ToString());
  }
}
