namespace Relay.SharedKernel;

public sealed record Result<T> : Result
{
    public T? Value { get; }

    private Result(T value) : base(true)
    {
        if (value is null)
        {
            throw new ArgumentException("A typed successful result must contain a value.",
                nameof(value));
        }

        Value = value;
    }

    private Result(Error error) : base(false, error)
    {
    }

    public static implicit operator Result<T>(T value) => new(value);

    public static implicit operator Result<T>(Error error) => new(error);
}
