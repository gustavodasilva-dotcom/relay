using Microsoft.AspNetCore.Routing;

namespace Relay.Routing.Abstractions;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder builder);
}
