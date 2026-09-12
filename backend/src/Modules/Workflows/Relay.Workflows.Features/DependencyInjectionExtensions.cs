using Microsoft.Extensions.DependencyInjection;
using Relay.Workflows.Features.GetPaginated;

namespace Relay.Workflows.Features;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddWorkflowsFeatures(
        this IServiceCollection services)
    {
        services.AddValidation();

        services.AddScoped<GetPaginatedHandler>();

        return services;
    }
}
