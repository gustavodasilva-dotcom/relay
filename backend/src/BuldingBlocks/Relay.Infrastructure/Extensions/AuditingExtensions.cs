using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Relay.Infrastructure.Abstractions;
using Relay.Infrastructure.Interceptors;

namespace Relay.Infrastructure.Extensions;

public static class AuditingExtensions
{
    public static IServiceCollection AddAuditingDbContext<TContext>(
        this IServiceCollection services,
        Action<IServiceProvider, DbContextOptionsBuilder> configure)
        where TContext : AuditingDbContext<TContext>
    {
        services.TryAddScoped<AuditLoggingInterceptor>();

        services.AddDbContext<TContext>((sp, options) =>
        {
            configure(sp, options);

            options.AddInterceptors(
                sp.GetRequiredService<AuditLoggingInterceptor>());
        });

        return services;
    }
}
