using Microsoft.EntityFrameworkCore;
using pattern_project.Database;
using pattern_project.Models;

namespace pattern_project.Repositories;

public class CategoryRepository(AppDbContext dbContext) : ICategoryRepository
{
  public async Task<List<Category>> GetAllAsync()
  {
    return await dbContext.Categories
      .AsNoTracking()
      .OrderBy(category => category.Name)
      .ToListAsync();
  }

  public async Task<bool> ExistsByIdAsync(int categoryId)
  {
    return await dbContext.Categories
      .AsNoTracking()
      .AnyAsync(category => category.Id == categoryId);
  }
}