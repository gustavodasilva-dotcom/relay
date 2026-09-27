using Microsoft.Extensions.Caching.Hybrid;
using Relay.SharedKernel;
using Relay.Workflows.Domain.Entities;
using Relay.Workflows.Domain.ValueObjects;
using Relay.Workflows.Features.Shared;
using Relay.Workflows.Infrastructure.Data;

namespace Relay.Workflows.Features.Create;

internal sealed class CreateHandler(
    HybridCache cache,
    WorkflowsDbContext dbContext,
    TimeProvider timeProvider)
{
    public async Task<Result<WorkflowResponse>> Handle(
        CreateRequest request,
        CancellationToken cancellationToken = default)
    {
        var nameResult = WorkflowName.Create(request.Name);

        if (!nameResult.IsSuccess)
        {
            return nameResult.Error;
        }

        var workflow = new Workflow(
            nameResult.Value!,
            timeProvider.GetUtcNow());

        dbContext.Workflows.Add(workflow);

        await dbContext.SaveChangesAsync(cancellationToken);

        var response = new WorkflowResponse
        {
            Id = workflow.Id,
            Name = workflow.Name.Value,
            Active = workflow.Active,
            CreatedAt = workflow.CreatedAt,
            UpdatedAt = workflow.UpdatedAt
        };

        await cache.SetAsync(
            key: $"workflow:{response.Id}",
            value: response,
            cancellationToken: cancellationToken);

        return response;
    }
}
