namespace Thb.OmnivoreClient;

public sealed record Result<T>
{
    private Result(T value)
    {
        Value = value;
        IsSuccess = true;
        Errors = default;
    }

    private Result(string[] errors)
    {
        Value = default;
        IsSuccess = false;
        Errors = errors;
    }

    public static Result<T> Success(T value) => new(value);

    public static Result<T> Failure(string[] errors) => new(errors);

    public T? Value { get; }
    public bool IsSuccess { get; }
    public string[]? Errors { get; }

    // ReSharper disable once UnusedMember.Global
    public void Deconstruct(out T? value, out bool isSuccess, out string[]? errors)
    {
        value = Value;
        isSuccess = IsSuccess;
        errors = Errors;
    }
}