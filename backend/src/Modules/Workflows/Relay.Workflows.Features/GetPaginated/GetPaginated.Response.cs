using System.Text.Json.Serialization;

namespace Relay.Workflows.Features.GetPaginated;

public sealed class GetPaginatedResponse
{
    public required Guid Id { get; init; }
    
    public required string Name { get; init; }
    
    public required bool Active { get; init; }

    [JsonPropertyName("created_at")]
    public required DateTimeOffset CreatedAt { get; init; }

    [JsonPropertyName("updated_at")]
    public required DateTimeOffset? UpdatedAt { get; init; }
}
