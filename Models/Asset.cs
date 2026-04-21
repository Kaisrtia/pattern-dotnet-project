namespace pattern_project.Models;

public abstract class Asset
{
  protected Asset()
  {
  }

  protected Asset(string name, decimal originalValue, string responsiblePerson)
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      throw new ArgumentException("Asset name is required.", nameof(name));
    }

    if (originalValue < 0)
    {
      throw new ArgumentOutOfRangeException(nameof(originalValue), "Original value cannot be negative.");
    }

    if (string.IsNullOrWhiteSpace(responsiblePerson))
    {
      throw new ArgumentException("Responsible person is required.", nameof(responsiblePerson));
    }

    Id = Guid.NewGuid();
    Name = name.Trim();
    OriginalValue = originalValue;
    ResponsiblePerson = responsiblePerson.Trim();
    CreatedAtUtc = DateTime.UtcNow;
  }

  public Guid Id { get; private set; }

  public string Name { get; private set; } = string.Empty;

  public decimal OriginalValue { get; private set; }

  public string ResponsiblePerson { get; private set; } = string.Empty;

  public DateTime CreatedAtUtc { get; private set; }

  public abstract AssetType AssetType { get; }

  public abstract decimal CalculateCurrentValue();
}
