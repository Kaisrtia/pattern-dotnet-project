using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using pattern_project.Domain.Exceptions;

namespace pattern_project.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
  private readonly ILogger<GlobalExceptionHandler> _logger;

  public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
  {
    _logger = logger;
  }

  public async ValueTask<bool> TryHandleAsync(
      HttpContext httpContext,
      Exception exception,
      CancellationToken cancellationToken)
  {
    _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

    var (statusCode, title) = exception switch
    {
      DomainValidationException => (StatusCodes.Status400BadRequest, "Bad request"),
      ForbiddenException => (StatusCodes.Status403Forbidden, "Forbidden"),
      NotFoundException => (StatusCodes.Status404NotFound, "Not found"),
      UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
      _ => (StatusCodes.Status500InternalServerError, "Server error")
    };

    var problemDetails = new ProblemDetails
    {
      Status = statusCode,
      Title = title,
      Detail = exception is DomainValidationException or ForbiddenException or NotFoundException or UnauthorizedAccessException
          ? exception.Message
          : "An unexpected error occurred. Please try again later."
    };

    httpContext.Response.StatusCode = statusCode;

    await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

    return true;
  }
}