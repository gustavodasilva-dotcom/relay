using Relay.Workflows.Domain.Entities;
using Relay.Workflows.Features.Shared;

namespace Relay.Workflows.Features.IntegrationTests.Assertions;

internal static class WorkflowResponseAssertions
{
    public static void ShouldMatch(
        this IReadOnlyList<WorkflowResponse> actual,
        IReadOnlyList<Workflow> expected)
    {
        Assert.Equal(expected.Count, actual.Count);

        for (var index = 0; index < expected.Count; index++)
        {
            actual[index].ShouldMatch(expected[index]);
        }
    }

    public static void ShouldMatch(
        this WorkflowResponse actual,
        Workflow expected)
    {
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.Name.Value, actual.Name);
        Assert.Equal(expected.Active, actual.Active);
        Assert.Equal(expected.CreatedAt, actual.CreatedAt);
        Assert.Equal(expected.UpdatedAt, actual.UpdatedAt);
    }
}
