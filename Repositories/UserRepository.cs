using Microsoft.EntityFrameworkCore;
using pattern_project.Database;
using pattern_project.Models;

namespace pattern_project.Repositories;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
  public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
  {
    return dbContext.Users
        .FirstOrDefaultAsync(u => u.Username == username && u.IsActive && !u.IsDeleted, cancellationToken);
  }

  public Task<User?> GetByIdAsync(long id, CancellationToken cancellationToken)
  {
    return dbContext.Users
        .FirstOrDefaultAsync(u => u.Id == id && u.IsActive && !u.IsDeleted, cancellationToken);
  }
}
