namespace pattern_project.Models;

public sealed class InterbankTransaction : Transaction
{
  private InterbankTransaction()
  {
  }

  public InterbankTransaction(
      long ownerUserId,
      decimal amount,
      long sourceAccountId,
      string swiftCode,
      string destinationBankName,
      string destinationAccountNumber,
      string? digitalSignature)
      : base(ownerUserId, amount)
  {
    SourceAccountId = sourceAccountId;
    SwiftCode = swiftCode;
    DestinationBankName = destinationBankName;
    DestinationAccountNumber = destinationAccountNumber;
    DigitalSignature = digitalSignature;
    RequiresDigitalSignature = amount > 50_000_000m;
  }

  public long SourceAccountId { get; private set; }
  public string SwiftCode { get; private set; } = string.Empty;
  public string DestinationBankName { get; private set; } = string.Empty;
  public string DestinationAccountNumber { get; private set; } = string.Empty;
  public string? DigitalSignature { get; private set; }
  public bool RequiresDigitalSignature { get; private set; }

  public Account SourceAccount { get; set; } = null!;
}
