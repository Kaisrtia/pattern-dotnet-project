using System.ComponentModel.DataAnnotations;

namespace pattern_project.Models;

public sealed class CreateCashAssetRequest
{
  [Required]
  [StringLength(120)]
  public required string Name { get; init; }

  [Range(typeof(decimal), "0", "999999999999999999")]
  public required decimal OriginalValue { get; init; }

  [Required]
  [StringLength(100)]
  public required string ResponsiblePerson { get; init; }
}

public sealed class CreateCryptoAssetRequest
{
  [Required]
  [StringLength(120)]
  public required string Name { get; init; }

  [Range(typeof(decimal), "0", "999999999999999999")]
  public required decimal OriginalValue { get; init; }

  [Range(typeof(decimal), "0", "999999")]
  public required decimal Multiplier { get; init; }

  [Required]
  [StringLength(100)]
  public required string ResponsiblePerson { get; init; }
}

public sealed class AssetView
{
  public required Guid Id { get; init; }
  public required string Name { get; init; }
  public required string ResponsiblePerson { get; init; }
  public required AssetType AssetType { get; init; }
  public required decimal OriginalValue { get; init; }
  public required decimal CurrentValue { get; init; }
  public decimal? Multiplier { get; init; }
  public required string DisplayLabel { get; init; }
  public required string DisplayColor { get; init; }
  public required DateTime CreatedAtUtc { get; init; }
}
