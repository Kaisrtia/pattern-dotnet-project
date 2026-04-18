using pattern_project.Dtos;

namespace pattern_project.Services;

public interface IProductService
{
  Task<IReadOnlyList<ProductResponseDto>> GetProductsAsync();
  Task<TotalInventoryValueDto> GetTotalInventoryValueAsync();
  Task<ServiceResult<ProductResponseDto>> CreateProductAsync(CreateUpdateProductRequestDto request);
  Task<ServiceResult<ProductResponseDto>> UpdateProductAsync(int id, CreateUpdateProductRequestDto request);
  Task<ServiceResult> DeleteProductAsync(int id);
}