namespace Lab13v11;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }

    private Result(T? value, bool isSuccess, string? error)
    {
        Value = value;
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result<T> Success(T value) => new(value, true, null);

    public static Result<T> Failure(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
            throw new ArgumentException("Опис помилки не може бути порожнім.", nameof(error));

        return new(default, false, error);
    }

    public static Result<T> Failure(Exception ex)
    {
        ArgumentNullException.ThrowIfNull(ex);
        return Failure($"{ex.GetType().Name}: {ex.Message}");
    }

    public override string ToString() => IsSuccess ? $"Success({Value})" : $"Failure({Error})";
}