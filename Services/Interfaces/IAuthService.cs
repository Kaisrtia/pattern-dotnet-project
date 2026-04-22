namespace pattern_project.Services.Interfaces;

public interface IAuthService
{
  Task<(int UserId, string Username, string Role)> ValidateLoginAsync(string username, string password, CancellationToken cancellationToken);
}
