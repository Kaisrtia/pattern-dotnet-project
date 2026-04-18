using Microsoft.AspNetCore.Mvc;
using pattern_project.Dtos;
using pattern_project.Services;

namespace pattern_project.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
  [HttpGet]
  public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetCategories()
  {
    var categories = await categoryService.GetCategoriesAsync();

    return Ok(categories);
  }
}