using pattern_project.Domain.Enums;

namespace pattern_project.Domain.Entities;

public class AppUser
{
  private AppUser()
  {
  }

  public AppUser(string username, string passwordHash, UserRole role)
  {
    Username = username;
    PasswordHash = passwordHash;
    Role = role;
  }

  public int Id { get; private set; }

  public string Username { get; private set; } = string.Empty;

  public string PasswordHash { get; private set; } = string.Empty;

  public UserRole Role { get; private set; }

  public ICollection<MedicalRecord> MedicalRecords { get; } = new List<MedicalRecord>();
}
