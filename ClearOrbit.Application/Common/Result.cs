namespace ClearOrbit.Application.Common;

public class Result<T>
{
    public bool Success { get; private set; }
    public string? Message { get; private set; }
    public T? Data { get; private set; }
    public List<string> Errors { get; private set; } = new();

    private Result(bool success, T? data, string? message, List<string>? errors)
    {
        Success = success;
        Data = data;
        Message = message;
        Errors = errors ?? new List<string>();
    }

    public static Result<T> Ok(T data, string? message = null)
        => new(true, data, message, null);

    public static Result<T> Fail(string message)
        => new(false, default, message, new List<string> { message });

    public static Result<T> Fail(List<string> errors)
        => new(false, default, "Validation failed", errors);
}