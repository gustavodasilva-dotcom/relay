using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Relay.SharedKernel;
using Relay.Workflows.Features.Errors;
using Relay.Workflows.Features.Shared;
using Relay.Workflows.Infrastructure.Data;

namespace Relay.Workflows.Features.GetById;

internal sealed class GetByIdHandler(
    HybridCache cache,
    WorkflowsDbContext dbContext)
{
    public async Task<Result<WorkflowResponse>> Handle(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var workflow = await cache.GetOrCreateAsync(
            key: $"workflow:{id}",
            factory: async ct => await dbContext.Workflows
                .AsNoTracking()
                .Where(workflow => workflow.Id == id)
                .Select(WorkflowResponse.Projection)
                .SingleOrDefaultAsync(ct),
            cancellationToken: cancellationToken);

        if (workflow is null)
        {
            return WorkflowErrors.NotFound(id);
        }

        return workflow;
    }
}
