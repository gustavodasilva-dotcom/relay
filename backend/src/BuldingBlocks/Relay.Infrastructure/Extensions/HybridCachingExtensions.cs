using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Relay.Infrastructure.Options;

namespace Relay.Infrastructure.Extensions;

public static class HybridCachingExtensions
{
    public static IServiceCollection AddHybridRedisCache(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<RedisCacheOptions> configure)
    {
        services.AddOptions<HybridCachingOptions>()
            .Bind(configuration.GetRequiredSection(
                HybridCachingOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddStackExchangeRedisCache(options => configure(options));

        services.AddHybridCache();

        services.AddOptions<HybridCacheOptions>()
            .Configure<IOptions<HybridCachingOptions>>((options, settings) =>
            {
                var values = settings.Value;

                options.MaximumPayloadBytes = values.MaximumPayloadBytes;
                options.MaximumKeyLength = values.MaximumKeyLength;
                options.DefaultEntryOptions = new HybridCacheEntryOptions
                {
                    LocalCacheExpiration = values.LocalCacheExpiration,
                    Expiration = values.Expiration
                };
                options.ReportTagMetrics = values.ReportTagMetrics;
            });

        return services;
    }
}
