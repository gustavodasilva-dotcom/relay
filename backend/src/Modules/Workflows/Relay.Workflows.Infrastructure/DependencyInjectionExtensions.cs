using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Relay.Infrastructure.Extensions;
using Relay.Workflows.Infrastructure.Data;

namespace Relay.Workflows.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddWorkflowsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddAuditingDbContext<WorkflowsDbContext>(
            (sp, options) =>
            {
                var configuration =
                    sp.GetRequiredService<IConfiguration>();

                options.UseNpgsql(
                    configuration.GetConnectionString("Postgres"));
            });

        services.AddHybridRedisCache(
            configuration,
            (options) =>
            {
                options.Configuration =
                    configuration.GetConnectionString("Redis");

                options.InstanceName =
                    $"relay:workflows:{environment.EnvironmentName}:";
            });

        return services;
    }

    public static void UseWorkflowsInfrastructure(this IHost host)
    {
        host.Migrate<WorkflowsDbContext>();
    }
}
