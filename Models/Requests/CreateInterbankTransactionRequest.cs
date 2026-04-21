using System.ComponentModel.DataAnnotations;

namespace pattern_project.Models.Requests;

public class CreateInterbankTransactionRequest
{
  [Range(1, double.MaxValue)]
  public decimal Amount { get; set; }

  [Range(1, long.MaxValue)]
  public long SourceAccountId { get; set; }

  [Required]
  [RegularExpression("^[A-Za-z0-9]{8}([A-Za-z0-9]{3})?$")]
  public string SwiftCode { get; set; } = string.Empty;

  [Required]
  [MaxLength(200)]
  public string DestinationBankName { get; set; } = string.Empty;

  [Required]
  [MaxLength(50)]
  public string DestinationAccountNumber { get; set; } = string.Empty;

  [MaxLength(500)]
  public string? DigitalSignature { get; set; }
}
