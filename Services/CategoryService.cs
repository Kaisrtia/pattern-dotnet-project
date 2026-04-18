using pattern_project.Dtos;
using pattern_project.Repositories;

namespace pattern_project.Services;

public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
  public async Task<IReadOnlyList<CategoryResponseDto>> GetCategoriesAsync()
  {
    var categories = await categoryRepository.GetAllAsync();

    return categories
      .Select(category => new CategoryResponseDto(category.Id, category.Name))
      .ToList();
  }
}