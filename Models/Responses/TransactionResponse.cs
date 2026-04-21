namespace pattern_project.Models.Responses;

public class TransactionResponse
{
  public required Guid Id { get; init; }
  public required decimal Amount { get; init; }
  public required string TransactionType { get; init; }
  public required long OwnerUserId { get; init; }
  public required DateTime CreatedAt { get; init; }
  public bool RequiresDigitalSignature { get; init; }

  public long? SourceAccountId { get; init; }
  public long? DestinationAccountId { get; init; }
  public string? SwiftCode { get; init; }
  public string? DestinationBankName { get; init; }
  public string? DestinationAccountNumber { get; init; }
  public bool HasDigitalSignature { get; init; }
}
