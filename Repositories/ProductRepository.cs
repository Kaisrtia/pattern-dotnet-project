using Microsoft.EntityFrameworkCore;
using pattern_project.Database;
using pattern_project.Models;

namespace pattern_project.Repositories;

public class ProductRepository(AppDbContext dbContext) : IProductRepository
{
  public async Task<List<Product>> GetAllWithCategoryAsync()
  {
    return await dbContext.Products
      .AsNoTracking()
      .Include(product => product.Category)
      .OrderBy(product => product.Name)
      .ToListAsync();
  }

  public async Task<decimal> GetTotalInventoryValueAsync()
  {
    return await dbContext.Products
      .AsNoTracking()
      .SumAsync(product => product.Price * product.Quantity);
  }

  public async Task<Product?> GetByIdAsync(int id)
  {
    return await dbContext.Products
      .FirstOrDefaultAsync(product => product.Id == id);
  }

  public async Task<Product?> GetByIdWithCategoryAsync(int id)
  {
    return await dbContext.Products
      .AsNoTracking()
      .Include(product => product.Category)
      .FirstOrDefaultAsync(product => product.Id == id);
  }

  public async Task AddAsync(Product product)
  {
    await dbContext.Products.AddAsync(product);
  }

  public async Task SaveChangesAsync()
  {
    await dbContext.SaveChangesAsync();
  }

  public void Remove(Product product)
  {
    dbContext.Products.Remove(product);
  }
}