namespace Relay.SharedKernel;

public record Result
{
    public bool IsSuccess { get; }

    public Error? Error { get; }

    protected Result(bool isSuccess, Error? error = null)
    {
        if (isSuccess is true && error is not null)
        {
            throw new ArgumentException("A successful result cannot contain an error.",
                nameof(error));
        }

        if (isSuccess is false && error is null)
        {
            throw new ArgumentException("A failed result must contain an error.",
                nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true);

    public static Result Failure(Error error) => new(false, error);

    public static implicit operator Result(Error error) => Failure(error);
}
