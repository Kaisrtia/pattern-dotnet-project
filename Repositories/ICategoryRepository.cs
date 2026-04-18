using pattern_project.Models;

namespace pattern_project.Repositories;

public interface ICategoryRepository
{
  Task<List<Category>> GetAllAsync();
  Task<bool> ExistsByIdAsync(int categoryId);
}