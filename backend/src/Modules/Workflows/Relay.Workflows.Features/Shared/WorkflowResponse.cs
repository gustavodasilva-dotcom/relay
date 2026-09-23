using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Relay.Workflows.Domain.Entities;

namespace Relay.Workflows.Features.Shared;

public sealed class WorkflowResponse
{
    internal static Expression<Func<Workflow, WorkflowResponse>> Projection { get; } =
        workflow => new WorkflowResponse
        {
            Id = workflow.Id,
            Name = workflow.Name.Value,
            Active = workflow.Active,
            CreatedAt = workflow.CreatedAt,
            UpdatedAt = workflow.UpdatedAt
        };

    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required bool Active { get; init; }

    [JsonPropertyName("created_at")]
    public required DateTimeOffset CreatedAt { get; init; }

    [JsonPropertyName("updated_at")]
    public required DateTimeOffset? UpdatedAt { get; init; }
}
