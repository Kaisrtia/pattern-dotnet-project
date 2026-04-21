namespace pattern_project.Services.Exceptions;

public class ForbiddenException(string message) : Exception(message)
{
}
