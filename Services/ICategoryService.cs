using pattern_project.Dtos;

namespace pattern_project.Services;

public interface ICategoryService
{
  Task<IReadOnlyList<CategoryResponseDto>> GetCategoriesAsync();
}