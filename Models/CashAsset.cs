namespace pattern_project.Models;

public sealed class CashAsset : Asset
{
  private CashAsset()
  {
  }

  public CashAsset(string name, decimal originalValue, string responsiblePerson)
      : base(name, originalValue, responsiblePerson)
  {
  }

  public override AssetType AssetType => AssetType.Cash;

  public override decimal CalculateCurrentValue() => OriginalValue;
}
