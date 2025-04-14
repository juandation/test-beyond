using TodoListApp.Core.Entities;

namespace TodoListApp.Core.Results;

public record ServiceResult(bool IsSuccess, string? ErrorMessage = null)
{
    public static ServiceResult Success() => new(true);
    public static ServiceResult Failure(string message) => new(false, message);
}

public record ServiceResult<T>(T? Value, bool IsSuccess, string? ErrorMessage = null) : ServiceResult(IsSuccess, ErrorMessage)
{
    public static ServiceResult<T> Success(T value) => new(value, true);
    public static new ServiceResult<T> Failure(string message) => new(default, false, message);
}
