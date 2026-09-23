using Relay.Workflows.Domain.Entities;
using Relay.Workflows.Domain.ValueObjects;

namespace Relay.Workflows.Features.IntegrationTests.Fixtures;

internal static class WorkflowFactory
{
    public static Workflow Create(int index = 1)
    {
        var name = WorkflowName.Create($"Workflow {index}").Value!;
        var createdAt = new DateTimeOffset(
            2026,
            1,
            index,
            12,
            0,
            0,
            TimeSpan.Zero);

        return new(name, createdAt);
    }
}
