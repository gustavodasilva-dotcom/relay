using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Relay.Routing.Abstractions;

namespace Relay.Workflows.Features.GetPaginated;

internal sealed class GetPaginatedEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet(
            "workflows",
            async (
                GetPaginatedHandler handler,
                [AsParameters] GetPaginatedRequest request,
                CancellationToken cancellationToken) =>
            {
                var response = await handler.Handle(request, cancellationToken);

                return Results.Ok(response);
            });
    }
}
