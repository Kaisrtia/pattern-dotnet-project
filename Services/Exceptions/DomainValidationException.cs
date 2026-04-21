namespace pattern_project.Services.Exceptions;

public class DomainValidationException(string message) : Exception(message)
{
}
