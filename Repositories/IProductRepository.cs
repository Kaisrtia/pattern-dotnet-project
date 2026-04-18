using pattern_project.Models;

namespace pattern_project.Repositories;

public interface IProductRepository
{
  Task<List<Product>> GetAllWithCategoryAsync();
  Task<decimal> GetTotalInventoryValueAsync();
  Task<Product?> GetByIdAsync(int id);
  Task<Product?> GetByIdWithCategoryAsync(int id);
  Task AddAsync(Product product);
  Task SaveChangesAsync();
  void Remove(Product product);
}