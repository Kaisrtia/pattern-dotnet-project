using pattern_project.Models;

namespace pattern_project.Repositories;

public interface IAccountRepository
{
  Task<Account?> GetByIdAsync(long id, CancellationToken cancellationToken);
}
