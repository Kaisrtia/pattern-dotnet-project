namespace pattern_project.Services;

public interface IUserContextService
{
  long GetRequiredUserId();
  bool IsAdmin();
}
