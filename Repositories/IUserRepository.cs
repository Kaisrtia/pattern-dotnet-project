using pattern_project.Models;

namespace pattern_project.Repositories;

public interface IUserRepository
{
  Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken);
  Task<User?> GetByIdAsync(long id, CancellationToken cancellationToken);
}
