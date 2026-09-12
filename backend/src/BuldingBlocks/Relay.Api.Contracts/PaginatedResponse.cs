using System.Text.Json.Serialization;

namespace Relay.Api.Contracts;

public sealed record PaginatedResponse<T>(
    IReadOnlyList<T> Items,
    int Page,
    [property: JsonPropertyName("page_size")]
    int PageSize,
    [property: JsonPropertyName("total_count")]
    int TotalCount)
{
    [JsonPropertyName("total_pages")]
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
