using Microsoft.EntityFrameworkCore;
using pattern_project.Database;
using pattern_project.Domain.Entities;
using pattern_project.Repositories.Interfaces;

namespace pattern_project.Repositories;

public sealed class UserRepository : IUserRepository
{
  private readonly AppDbContext _dbContext;

  public UserRepository(AppDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public Task<AppUser?> FindByUsernameAsync(string username, CancellationToken cancellationToken)
  {
    return _dbContext.Users.FirstOrDefaultAsync(x => x.Username == username, cancellationToken);
  }

  public Task<AppUser?> FindByIdAsync(int userId, CancellationToken cancellationToken)
  {
    return _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
  }
}
