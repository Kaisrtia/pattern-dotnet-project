using Microsoft.EntityFrameworkCore;
using pattern_project.Database;
using pattern_project.Models;

namespace pattern_project.Repositories;

public class AccountRepository(AppDbContext dbContext) : IAccountRepository
{
  public Task<Account?> GetByIdAsync(long id, CancellationToken cancellationToken)
  {
    return dbContext.Accounts
        .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted, cancellationToken);
  }
}
