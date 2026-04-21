namespace pattern_project.Models.Responses;

public class AuthResponse
{
  public required long UserId { get; init; }
  public required string Username { get; init; }
  public required string Role { get; init; }
}
