namespace pattern_project.Services.Exceptions;

public class NotFoundException(string message) : Exception(message)
{
}
