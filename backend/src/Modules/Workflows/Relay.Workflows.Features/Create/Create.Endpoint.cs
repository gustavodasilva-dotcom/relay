using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Relay.Routing.Abstractions;
using Relay.Routing.Results;

namespace Relay.Workflows.Features.Create;

internal sealed class CreateEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost(
            "workflows",
            async (
                CreateHandler handler,
                [FromBody] CreateRequest request,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(request, cancellationToken);

                return result.Match(
                    value => Results.Created($"workflows/{value.Id}", value),
                    ApiResults.Problem);
            });
    }
}
