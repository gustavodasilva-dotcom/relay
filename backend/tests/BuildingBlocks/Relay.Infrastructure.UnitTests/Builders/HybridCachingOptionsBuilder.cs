using Relay.Infrastructure.Options;

namespace Relay.Infrastructure.UnitTests.Builders;

internal sealed class HybridCachingOptionsBuilder
{
    private long _maximumPayloadBytes = 1024 * 1024;
    private int _maximumKeyLength = 256;
    private TimeSpan _localCacheExpiration = TimeSpan.FromMinutes(5);
    private TimeSpan _expiration = TimeSpan.FromMinutes(30);
    private bool _reportTagMetrics = true;

    public HybridCachingOptionsBuilder WithMaximumPayloadBytes(long maximumPayloadBytes)
    {
        _maximumPayloadBytes = maximumPayloadBytes;

        return this;
    }

    public HybridCachingOptionsBuilder WithMaximumKeyLength(int maximumKeyLength)
    {
        _maximumKeyLength = maximumKeyLength;

        return this;
    }

    public HybridCachingOptionsBuilder WithLocalCacheExpiration(TimeSpan localCacheExpiration)
    {
        _localCacheExpiration = localCacheExpiration;

        return this;
    }

    public HybridCachingOptionsBuilder WithExpiration(TimeSpan expiration)
    {
        _expiration = expiration;

        return this;
    }

    public HybridCachingOptionsBuilder WithReportTagMetrics(bool reportTagMetrics)
    {
        _reportTagMetrics = reportTagMetrics;

        return this;
    }

    public HybridCachingOptions Build() =>
        new()
        {
            MaximumPayloadBytes = _maximumPayloadBytes,
            MaximumKeyLength = _maximumKeyLength,
            LocalCacheExpiration = _localCacheExpiration,
            Expiration = _expiration,
            ReportTagMetrics = _reportTagMetrics
        };
}
