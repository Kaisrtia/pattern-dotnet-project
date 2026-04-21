using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using pattern_project.Services.Exceptions;

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
      DomainValidationException => (StatusCodes.Status400BadRequest, "Validation error"),
      ForbiddenException => (StatusCodes.Status403Forbidden, "Forbidden"),
      NotFoundException => (StatusCodes.Status404NotFound, "Not found"),
      _ => (StatusCodes.Status500InternalServerError, "Server error")
    };

    var problemDetails = new ProblemDetails
    {
      Status = statusCode,
      Title = title,
      Detail = exception.Message
    };

    httpContext.Response.StatusCode = statusCode;

    await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

    return true;
  }
}