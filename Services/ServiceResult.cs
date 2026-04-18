namespace pattern_project.Services;

public sealed record ServiceResult<T>(bool IsSuccess, T? Value, int StatusCode, string? ErrorMessage)
{
  public static ServiceResult<T> Success(T value, int statusCode = StatusCodes.Status200OK)
  {
    return new ServiceResult<T>(true, value, statusCode, null);
  }

  public static ServiceResult<T> Failure(string message, int statusCode)
  {
    return new ServiceResult<T>(false, default, statusCode, message);
  }
}

public sealed record ServiceResult(bool IsSuccess, int StatusCode, string? ErrorMessage)
{
  public static ServiceResult Success(int statusCode = StatusCodes.Status204NoContent)
  {
    return new ServiceResult(true, statusCode, null);
  }

  public static ServiceResult Failure(string message, int statusCode)
  {
    return new ServiceResult(false, statusCode, message);
  }
}