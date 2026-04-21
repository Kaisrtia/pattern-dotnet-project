namespace pattern_project.Models;

public abstract class Transaction
{
  protected Transaction()
  {
  }

  protected Transaction(long ownerUserId, decimal amount)
  {
    OwnerUserId = ownerUserId;
    Amount = amount;
  }

  public Guid Id { get; init; } = Guid.NewGuid();
  public decimal Amount { get; init; }
  public long OwnerUserId { get; private set; }
  public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
  public bool IsDeleted { get; private set; }
  public DateTime? DeletedAt { get; private set; }

  public User OwnerUser { get; set; } = null!;

  public void SoftDelete()
  {
    IsDeleted = true;
    DeletedAt = DateTime.UtcNow;
  }
}
