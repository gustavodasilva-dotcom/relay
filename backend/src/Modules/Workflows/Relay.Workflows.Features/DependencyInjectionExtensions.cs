using Microsoft.Extensions.DependencyInjection;
using Relay.Workflows.Features.Create;
using Relay.Workflows.Features.GetById;
using Relay.Workflows.Features.GetPaginated;

namespace Relay.Workflows.Features;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddWorkflowsFeatures(
        this IServiceCollection services)
    {
        services.AddValidation();

        services
            .AddScoped<CreateHandler>()
            .AddScoped<GetByIdHandler>()
            .AddScoped<GetPaginatedHandler>();

        return services;
    }
}
