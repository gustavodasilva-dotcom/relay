using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Relay.Routing.Abstractions;
using Relay.Routing.Results;

namespace Relay.Workflows.Features.GetById;

internal sealed class GetByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet(
            "workflows/{id:guid}",
            async (
                GetByIdHandler handler,
                Guid id,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(id, cancellationToken);

                return result.Match(
                    value => Results.Ok(value),
                    ApiResults.Problem);
            });
    }
}
