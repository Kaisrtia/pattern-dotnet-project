using Microsoft.AspNetCore.Mvc;
using pattern_project.Dtos;
using pattern_project.Services;

namespace pattern_project.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController(IProductService productService) : ControllerBase
{
  [HttpGet]
  public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetProducts()
  {
    var products = await productService.GetProductsAsync();

    return Ok(products);
  }

  [HttpGet("total-inventory-value")]
  public async Task<ActionResult<TotalInventoryValueDto>> GetTotalInventoryValue()
  {
    var totalInventoryValue = await productService.GetTotalInventoryValueAsync();
    return Ok(totalInventoryValue);
  }

  [HttpPost]
  public async Task<ActionResult<ProductResponseDto>> CreateProduct(CreateUpdateProductRequestDto request)
  {
    var result = await productService.CreateProductAsync(request);
    if (!result.IsSuccess || result.Value is null)
    {
      return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }

    return CreatedAtAction(nameof(GetProducts), new { id = result.Value.Id }, result.Value);
  }

  [HttpPut("{id:int}")]
  public async Task<ActionResult<ProductResponseDto>> UpdateProduct(int id, CreateUpdateProductRequestDto request)
  {
    var result = await productService.UpdateProductAsync(id, request);
    if (!result.IsSuccess || result.Value is null)
    {
      return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }

    return Ok(result.Value);
  }

  [HttpDelete("{id:int}")]
  public async Task<IActionResult> DeleteProduct(int id)
  {
    var result = await productService.DeleteProductAsync(id);
    if (!result.IsSuccess)
    {
      return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }

    return NoContent();
  }
}