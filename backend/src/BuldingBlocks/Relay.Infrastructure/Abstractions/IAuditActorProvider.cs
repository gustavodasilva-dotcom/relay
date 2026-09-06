namespace Relay.Infrastructure.Abstractions;

public interface IAuditActorProvider
{
    string? GetActorId();
}
