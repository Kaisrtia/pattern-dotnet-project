using pattern_project.Dtos;
using pattern_project.Models;
using pattern_project.Repositories;

namespace pattern_project.Services;

public class ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository) : IProductService
{
  public async Task<IReadOnlyList<ProductResponseDto>> GetProductsAsync()
  {
    var products = await productRepository.GetAllWithCategoryAsync();

    return products
      .Select(MapToProductResponse)
      .ToList();
  }

  public async Task<TotalInventoryValueDto> GetTotalInventoryValueAsync()
  {
    var total = await productRepository.GetTotalInventoryValueAsync();
    return new TotalInventoryValueDto(total);
  }

  public async Task<ServiceResult<ProductResponseDto>> CreateProductAsync(CreateUpdateProductRequestDto request)
  {
    var validationError = ValidateProductInput(request);
    if (validationError is not null)
    {
      return ServiceResult<ProductResponseDto>.Failure(validationError, StatusCodes.Status400BadRequest);
    }

    var categoryExists = await categoryRepository.ExistsByIdAsync(request.CategoryId);
    if (!categoryExists)
    {
      return ServiceResult<ProductResponseDto>.Failure("CategoryId does not exist.", StatusCodes.Status400BadRequest);
    }

    var product = new Product
    {
      Name = request.Name.Trim(),
      Price = request.Price,
      Quantity = request.Quantity,
      CategoryId = request.CategoryId
    };

    await productRepository.AddAsync(product);
    await productRepository.SaveChangesAsync();

    var createdProduct = await productRepository.GetByIdWithCategoryAsync(product.Id);
    if (createdProduct is null)
    {
      return ServiceResult<ProductResponseDto>.Failure("Product not found.", StatusCodes.Status404NotFound);
    }

    return ServiceResult<ProductResponseDto>.Success(MapToProductResponse(createdProduct), StatusCodes.Status201Created);
  }

  public async Task<ServiceResult<ProductResponseDto>> UpdateProductAsync(int id, CreateUpdateProductRequestDto request)
  {
    var validationError = ValidateProductInput(request);
    if (validationError is not null)
    {
      return ServiceResult<ProductResponseDto>.Failure(validationError, StatusCodes.Status400BadRequest);
    }

    var product = await productRepository.GetByIdAsync(id);
    if (product is null)
    {
      return ServiceResult<ProductResponseDto>.Failure("Product not found.", StatusCodes.Status404NotFound);
    }

    var categoryExists = await categoryRepository.ExistsByIdAsync(request.CategoryId);
    if (!categoryExists)
    {
      return ServiceResult<ProductResponseDto>.Failure("CategoryId does not exist.", StatusCodes.Status400BadRequest);
    }

    product.Name = request.Name.Trim();
    product.Price = request.Price;
    product.Quantity = request.Quantity;
    product.CategoryId = request.CategoryId;

    await productRepository.SaveChangesAsync();

    var updatedProduct = await productRepository.GetByIdWithCategoryAsync(product.Id);
    if (updatedProduct is null)
    {
      return ServiceResult<ProductResponseDto>.Failure("Product not found.", StatusCodes.Status404NotFound);
    }

    return ServiceResult<ProductResponseDto>.Success(MapToProductResponse(updatedProduct));
  }

  public async Task<ServiceResult> DeleteProductAsync(int id)
  {
    var product = await productRepository.GetByIdAsync(id);
    if (product is null)
    {
      return ServiceResult.Failure("Product not found.", StatusCodes.Status404NotFound);
    }

    productRepository.Remove(product);
    await productRepository.SaveChangesAsync();

    return ServiceResult.Success();
  }

  private static string? ValidateProductInput(CreateUpdateProductRequestDto request)
  {
    if (string.IsNullOrWhiteSpace(request.Name))
    {
      return "Name is required.";
    }

    if (request.Price <= 0)
    {
      return "Price must be greater than 0.";
    }

    if (request.Quantity <= 0)
    {
      return "Quantity must be greater than 0.";
    }

    if (request.CategoryId <= 0)
    {
      return "CategoryId must be greater than 0.";
    }

    return null;
  }

  private static ProductResponseDto MapToProductResponse(Product product)
  {
    return new ProductResponseDto(
      product.Id,
      product.Name,
      product.Price,
      product.Quantity,
      product.CategoryId,
      product.Category?.Name ?? string.Empty
    );
  }
}