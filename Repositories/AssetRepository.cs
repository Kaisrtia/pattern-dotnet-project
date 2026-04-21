using Microsoft.EntityFrameworkCore;
using pattern_project.Database;
using pattern_project.Models;

namespace pattern_project.Repositories;

public sealed class AssetRepository : IAssetRepository
{
  private readonly AppDbContext _dbContext;

  public AssetRepository(AppDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task<IReadOnlyList<Asset>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    return await _dbContext.Assets
        .AsNoTracking()
        .OrderByDescending(asset => asset.CreatedAtUtc)
        .ToListAsync(cancellationToken);
  }

  public async Task<Asset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
  {
    return await _dbContext.Assets
        .AsNoTracking()
        .FirstOrDefaultAsync(asset => asset.Id == id, cancellationToken);
  }

  public async Task AddAsync(Asset asset, CancellationToken cancellationToken = default)
  {
    await _dbContext.Assets.AddAsync(asset, cancellationToken);
    await _dbContext.SaveChangesAsync(cancellationToken);
  }
}
