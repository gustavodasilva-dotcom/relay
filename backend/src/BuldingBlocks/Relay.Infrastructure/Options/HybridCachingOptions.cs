using System.ComponentModel.DataAnnotations;

namespace Relay.Infrastructure.Options;

public sealed class HybridCachingOptions : IValidatableObject
{
    public const string SectionName = "Caching:Hybrid";

    public long MaximumPayloadBytes { get; init; }

    public int MaximumKeyLength { get; init; }

    public TimeSpan LocalCacheExpiration { get; init; }

    public TimeSpan Expiration { get; init; }

    public bool ReportTagMetrics { get; init; }

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        if (MaximumPayloadBytes <= 0)
        {
            yield return new ValidationResult(
                "\"MaximumPayloadBytes\" must be greater than zero.",
                [nameof(MaximumPayloadBytes)]);
        }

        if (MaximumKeyLength <= 0)
        {
            yield return new ValidationResult(
                "\"MaximumKeyLength\" must be greater than zero.",
                [nameof(MaximumKeyLength)]);
        }

        if (LocalCacheExpiration <= TimeSpan.Zero)
        {
            yield return new ValidationResult(
                "\"LocalCacheExpiration\" must be greater than zero.",
                [nameof(LocalCacheExpiration)]);
        }

        if (Expiration <= TimeSpan.Zero)
        {
            yield return new ValidationResult(
                "\"Expiration\" must be greater than zero.",
                [nameof(Expiration)]);
        }

        if (LocalCacheExpiration > TimeSpan.Zero
            && Expiration > TimeSpan.Zero
            && LocalCacheExpiration > Expiration)
        {
            yield return new ValidationResult(
                "\"LocalCacheExpiration\" cannot exceed \"Expiration\".",
                [nameof(LocalCacheExpiration), nameof(Expiration)]);
        }
    }
}
