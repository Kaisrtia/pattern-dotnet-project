using pattern_project.Models;

namespace pattern_project.Services;

public interface IAssetService
{
  Task<IReadOnlyList<AssetView>> GetAllAsync(CancellationToken cancellationToken = default);

  Task<AssetView?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

  Task<AssetView> CreateCashAsync(CreateCashAssetRequest request, CancellationToken cancellationToken = default);

  Task<AssetView> CreateCryptoAsync(CreateCryptoAssetRequest request, CancellationToken cancellationToken = default);
}
