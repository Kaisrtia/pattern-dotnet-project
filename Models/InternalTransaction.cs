namespace pattern_project.Models;

public sealed class InternalTransaction : Transaction
{
  private InternalTransaction()
  {
  }

  public InternalTransaction(long ownerUserId, decimal amount, long sourceAccountId, long destinationAccountId)
      : base(ownerUserId, amount)
  {
    SourceAccountId = sourceAccountId;
    DestinationAccountId = destinationAccountId;
  }

  public long SourceAccountId { get; private set; }
  public long DestinationAccountId { get; private set; }

  public Account SourceAccount { get; set; } = null!;
  public Account DestinationAccount { get; set; } = null!;
}
