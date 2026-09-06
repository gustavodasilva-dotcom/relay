using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Relay.Infrastructure.Extensions;

namespace Relay.Workflows.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
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
}
