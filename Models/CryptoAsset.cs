namespace pattern_project.Models;

public sealed class CryptoAsset : Asset
{
  private decimal _multiplier;

  private CryptoAsset()
  {
  }

  public CryptoAsset(string name, decimal originalValue, decimal multiplier, string responsiblePerson)
      : base(name, originalValue, responsiblePerson)
  {
    Multiplier = multiplier;
  }

  public decimal Multiplier
  {
    get => _multiplier;
    private set
    {
      if (value < 0)
      {
        throw new ArgumentOutOfRangeException(nameof(Multiplier), "Multiplier cannot be negative.");
      }

      _multiplier = value;
    }
  }

  public override AssetType AssetType => AssetType.Crypto;

  public override decimal CalculateCurrentValue() => OriginalValue * Multiplier;
}
