namespace pattern_project.Models;

public class User
{
  public long Id { get; set; }
  public required string Username { get; set; }
  public required string Email { get; set; }
  public required string FullName { get; set; }
  public required string PasswordHash { get; set; }
  public UserRole Role { get; set; } = UserRole.User;
  public bool IsActive { get; set; } = true;
  public bool IsDeleted { get; set; } = false;
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public Account? Account { get; set; }
  public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
