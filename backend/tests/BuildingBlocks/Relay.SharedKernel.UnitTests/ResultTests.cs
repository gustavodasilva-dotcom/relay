namespace Relay.SharedKernel.UnitTests;

public sealed class ResultTests
{
    [Fact]
    public void Success_creates_successful_result_without_error()
    {
        // Arrange & Act
        var result = Result.Success();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Failure_creates_failed_result_when_error()
    {
        // Arrange
        var error = CreateError();

        // Act
        var result = Result.Failure(error);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void Error_implicitly_converts_to_failure_result()
    {
        // Arrange
        var error = CreateError();

        // Act
        Result result = error;

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void Error_implicitly_converts_to_value_typed_failure_result()
    {
        // Arrange
        var error = CreateError();

        // Act
        Result<int> result = error;

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
        Assert.Equal(default, result.Value);
    }

    [Fact]
    public void Error_implicitly_converts_to_reference_typed_failure_result()
    {
        // Arrange
        var error = CreateError();

        // Act
        Result<string> result = error;

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(error, result.Error);
        Assert.Null(result.Value);
    }

    [Fact]
    public void Typed_value_implicitly_converts_to_success_result()
    {
        // Arrange
        const int Value = 42;

        // Act
        Result<int> result = Value;

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(Value, result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Typed_result_rejects_null_value()
    {
        // Arrange
        string value = null!;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
        {
            Result<string> result = value;
        });
    }

    [Fact]
    public void Successful_result_cannot_contain_error()
    {
        Assert.Throws<ArgumentException>(() =>
            new ExposedResult(true, CreateError()));
    }

    [Fact]
    public void Failed_result_must_contain_error()
    {
        Assert.Throws<ArgumentException>(() =>
            new ExposedResult(false, null));
    }

    private static Error CreateError() =>
        new("Some title", "Some description");

    private sealed record ExposedResult : Result
    {
        public ExposedResult(bool isSuccess, Error? error)
            : base(isSuccess, error)
        {
        }
    }
}
