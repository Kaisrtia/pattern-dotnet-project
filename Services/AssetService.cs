using pattern_project.Models;
using pattern_project.Repositories;

namespace pattern_project.Services;

public sealed class AssetService : IAssetService
{
  private readonly IAssetRepository _assetRepository;

  public AssetService(IAssetRepository assetRepository)
  {
    _assetRepository = assetRepository;
  }

  public async Task<IReadOnlyList<AssetView>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    var assets = await _assetRepository.GetAllAsync(cancellationToken);
    return assets.Select(MapToView).ToList();
  }

  public async Task<AssetView?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
  {
    var asset = await _assetRepository.GetByIdAsync(id, cancellationToken);
    return asset is null ? null : MapToView(asset);
  }

  public async Task<AssetView> CreateCashAsync(CreateCashAssetRequest request, CancellationToken cancellationToken = default)
  {
    var asset = new CashAsset(
        request.Name,
        request.OriginalValue,
        request.ResponsiblePerson);

    await _assetRepository.AddAsync(asset, cancellationToken);
    return MapToView(asset);
  }

  public async Task<AssetView> CreateCryptoAsync(CreateCryptoAssetRequest request, CancellationToken cancellationToken = default)
  {
    if (request.Multiplier < 0)
    {
      throw new ArgumentOutOfRangeException(nameof(request.Multiplier), "Multiplier cannot be negative.");
    }

    var asset = new CryptoAsset(
        request.Name,
        request.OriginalValue,
        request.Multiplier,
        request.ResponsiblePerson);

    await _assetRepository.AddAsync(asset, cancellationToken);
    return MapToView(asset);
  }

  private static AssetView MapToView(Asset asset)
  {
    var currentValue = asset.CalculateCurrentValue();

    return asset switch
    {
      CashAsset => new AssetView
      {
        Id = asset.Id,
        Name = asset.Name,
        ResponsiblePerson = asset.ResponsiblePerson,
        AssetType = asset.AssetType,
        OriginalValue = asset.OriginalValue,
        CurrentValue = currentValue,
        Multiplier = null,
        DisplayLabel = "Cash",
        DisplayColor = "#0D9488",
        CreatedAtUtc = asset.CreatedAtUtc
      },
      CryptoAsset cryptoAsset => new AssetView
      {
        Id = cryptoAsset.Id,
        Name = cryptoAsset.Name,
        ResponsiblePerson = cryptoAsset.ResponsiblePerson,
        AssetType = cryptoAsset.AssetType,
        OriginalValue = cryptoAsset.OriginalValue,
        CurrentValue = currentValue,
        Multiplier = cryptoAsset.Multiplier,
        DisplayLabel = "Crypto",
        DisplayColor = "#DC2626",
        CreatedAtUtc = cryptoAsset.CreatedAtUtc
      },
      _ => throw new InvalidOperationException($"Unsupported asset type: {asset.GetType().Name}")
    };
  }
}
