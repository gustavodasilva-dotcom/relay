using System.Security.Claims;
using Relay.Infrastructure.Abstractions;

namespace Hosts.WebApi.Impl;

internal sealed class WebApiAuditActorProvider(
    IHttpContextAccessor httpContextAccessor) : IAuditActorProvider
{
    public string? GetActorId() =>
        httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
}
