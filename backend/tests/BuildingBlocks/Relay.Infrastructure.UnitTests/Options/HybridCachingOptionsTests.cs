using System.ComponentModel.DataAnnotations;
using Relay.Infrastructure.Options;
using Relay.Infrastructure.UnitTests.Builders;

namespace Relay.Infrastructure.UnitTests.Options;

public sealed class HybridCachingOptionsTests
{
    public static TheoryData<TimeSpan> NonPositiveDurations =>
        new(TimeSpan.FromTicks(-1), TimeSpan.Zero);

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Validate_WhenMaximumPayloadBytesIsNonPositive_ReturnsValidationError(long value)
    {
        var options = new HybridCachingOptionsBuilder()
            .WithMaximumPayloadBytes(value)
            .Build();

        AssertSingleFailure(
            options,
            "\"MaximumPayloadBytes\" must be greater than zero.",
            nameof(HybridCachingOptions.MaximumPayloadBytes));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(long.MaxValue)]
    public void Validate_WhenMaximumPayloadBytesIsPositive_Succeeds(long value)
    {
        var options = new HybridCachingOptionsBuilder()
            .WithMaximumPayloadBytes(value)
            .Build();

        AssertValid(options);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Validate_WhenMaximumKeyLengthIsNonPositive_ReturnsValidationError(int value)
    {
        var options = new HybridCachingOptionsBuilder()
            .WithMaximumKeyLength(value)
            .Build();

        AssertSingleFailure(
            options,
            "\"MaximumKeyLength\" must be greater than zero.",
            nameof(HybridCachingOptions.MaximumKeyLength));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(int.MaxValue)]
    public void Validate_WhenMaximumKeyLengthIsPositive_Succeeds(int value)
    {
        var options = new HybridCachingOptionsBuilder()
            .WithMaximumKeyLength(value)
            .Build();

        AssertValid(options);
    }

    [Theory]
    [MemberData(nameof(NonPositiveDurations))]
    public void Validate_WhenLocalCacheExpirationIsNonPositive_ReturnsValidationError(
        TimeSpan value)
    {
        var options = new HybridCachingOptionsBuilder()
            .WithLocalCacheExpiration(value)
            .Build();

        AssertSingleFailure(
            options,
            "\"LocalCacheExpiration\" must be greater than zero.",
            nameof(HybridCachingOptions.LocalCacheExpiration));
    }

    [Fact]
    public void Validate_WhenLocalCacheExpirationIsOneTick_Succeeds()
    {
        var options = new HybridCachingOptionsBuilder()
            .WithLocalCacheExpiration(TimeSpan.FromTicks(1))
            .Build();

        AssertValid(options);
    }

    [Theory]
    [MemberData(nameof(NonPositiveDurations))]
    public void Validate_WhenExpirationIsNonPositive_ReturnsValidationError(TimeSpan value)
    {
        var options = new HybridCachingOptionsBuilder()
            .WithExpiration(value)
            .Build();

        AssertSingleFailure(
            options,
            "\"Expiration\" must be greater than zero.",
            nameof(HybridCachingOptions.Expiration));
    }

    [Fact]
    public void Validate_WhenLocalCacheExpirationIsLessThanExpiration_Succeeds()
    {
        var options = new HybridCachingOptionsBuilder()
            .WithLocalCacheExpiration(TimeSpan.FromTicks(1))
            .WithExpiration(TimeSpan.FromTicks(2))
            .Build();

        AssertValid(options);
    }

    [Fact]
    public void Validate_WhenExpirationsAreEqual_Succeeds()
    {
        var options = new HybridCachingOptionsBuilder()
            .WithLocalCacheExpiration(TimeSpan.FromTicks(1))
            .WithExpiration(TimeSpan.FromTicks(1))
            .Build();

        AssertValid(options);
    }

    [Fact]
    public void Validate_WhenLocalCacheExpirationExceedsExpiration_ReturnsValidationError()
    {
        var options = new HybridCachingOptionsBuilder()
            .WithLocalCacheExpiration(TimeSpan.FromTicks(2))
            .WithExpiration(TimeSpan.FromTicks(1))
            .Build();

        AssertSingleFailure(
            options,
            "\"LocalCacheExpiration\" cannot exceed \"Expiration\".",
            nameof(HybridCachingOptions.LocalCacheExpiration),
            nameof(HybridCachingOptions.Expiration));
    }

    private static void AssertValid(HybridCachingOptions options)
    {
        var (isValid, results) = Validate(options);

        Assert.True(isValid);
        Assert.Empty(results);
    }

    private static void AssertSingleFailure(
        HybridCachingOptions options,
        string errorMessage,
        params string[] memberNames)
    {
        var (isValid, results) = Validate(options);

        Assert.False(isValid);

        var validationResult = Assert.Single(results);

        Assert.Equal(errorMessage, validationResult.ErrorMessage);
        Assert.Equal(memberNames, validationResult.MemberNames);
    }

    private static (bool IsValid, List<ValidationResult> Results) Validate(
        HybridCachingOptions options)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(options);
        var isValid = Validator.TryValidateObject(options, context, results);

        return (isValid, results);
    }
}
