using System.ComponentModel.DataAnnotations;

namespace pattern_project.Models.Requests;

public class CreateInternalTransactionRequest
{
  [Range(1, double.MaxValue)]
  public decimal Amount { get; set; }

  [Range(1, long.MaxValue)]
  public long SourceAccountId { get; set; }

  [Range(1, long.MaxValue)]
  public long DestinationAccountId { get; set; }
}
