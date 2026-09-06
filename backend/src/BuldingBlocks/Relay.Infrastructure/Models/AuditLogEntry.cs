namespace Relay.Infrastructure.Models;

public sealed class AuditLogEntry
{
    public Guid Id { get; init; }

    public required string EntityName { get; init; }

    public required string EntityId { get; init; }

    public required string Action { get; init; }

    public string? UserId { get; init; }

    public DateTimeOffset Timestamp { get; init; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string? AffectedColumns { get; set; }
}
