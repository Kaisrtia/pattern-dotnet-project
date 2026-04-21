using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

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

    if (exception is ArgumentOutOfRangeException argumentOutOfRangeException)
    {
      var validationProblem = new ValidationProblemDetails
      {
        Status = StatusCodes.Status400BadRequest,
        Title = "Validation failed",
        Detail = argumentOutOfRangeException.Message
      };

      validationProblem.Errors.Add(
          argumentOutOfRangeException.ParamName ?? "input",
          new[] { argumentOutOfRangeException.Message });

      httpContext.Response.StatusCode = validationProblem.Status.Value;
      await httpContext.Response.WriteAsJsonAsync(validationProblem, cancellationToken);
      return true;
    }

    var problemDetails = new ValidationProblemDetails
    {
      Status = StatusCodes.Status500InternalServerError,
      Title = "Server error",
      Detail = "An unexpected error occurred. Please try again later."
    };

    httpContext.Response.StatusCode = problemDetails.Status.Value;

    await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

    return true;
  }
}