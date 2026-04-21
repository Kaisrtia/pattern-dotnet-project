using pattern_project.Models;

namespace pattern_project.Repositories;

public interface IAssetRepository
{
  Task<IReadOnlyList<Asset>> GetAllAsync(CancellationToken cancellationToken = default);

  Task<Asset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

  Task AddAsync(Asset asset, CancellationToken cancellationToken = default);
}
