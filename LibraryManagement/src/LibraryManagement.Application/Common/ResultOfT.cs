namespace LibraryManagement.Application.Common;

public class Result<T> : Result
{
    private Result(bool isSuccess, T? value, string? error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    public T? Value { get; }

    public static Result<T> Success(T value)
    {
        return new Result<T>(true, value, null);
    }

    public new static Result<T> Failure(string error)
    {
        return new Result<T>(false, default, error);
    }
}
