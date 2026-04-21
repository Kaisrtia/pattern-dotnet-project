namespace pattern_project.Models;

public class Account
{
  public long Id { get; set; }
  public long UserId { get; set; }
  public required string AccountNumber { get; set; }
  public decimal Balance { get; set; }
  public bool IsDeleted { get; set; } = false;
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public User User { get; set; } = null!;
}
