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
        this IServiceCollection services)
    {
        services.AddAuditingDbContext<WorkflowsDbContext>(
            (sp, options) =>
            {
                var configuration =
                    sp.GetRequiredService<IConfiguration>();

                options.UseNpgsql(
                    configuration.GetConnectionString("Default"));
            });

        return services;
    }

    public static void UseWorkflowsInfrastructure(this IHost host)
    {
        host.Migrate<WorkflowsDbContext>();
    }
}
