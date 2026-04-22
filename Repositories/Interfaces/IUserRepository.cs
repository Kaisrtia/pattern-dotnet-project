using pattern_project.Domain.Entities;

namespace pattern_project.Repositories.Interfaces;

public interface IUserRepository
{
  Task<AppUser?> FindByUsernameAsync(string username, CancellationToken cancellationToken);

  Task<AppUser?> FindByIdAsync(int userId, CancellationToken cancellationToken);
}
