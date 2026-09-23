using Microsoft.EntityFrameworkCore;
using Relay.SharedKernel;
using Relay.Workflows.Features.Errors;
using Relay.Workflows.Features.Shared;
using Relay.Workflows.Infrastructure.Data;

namespace Relay.Workflows.Features.GetById;

internal sealed class GetByIdHandler(WorkflowsDbContext dbContext)
{
    public async Task<Result<WorkflowResponse>> Handle(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var workflow = await dbContext.Workflows
            .Where(workflow => workflow.Id == id)
            .Select(WorkflowResponse.Projection)
            .SingleOrDefaultAsync(cancellationToken);

        if (workflow is null)
        {
            return WorkflowErrors.NotFound(id);
        }

        return workflow;
    }
}
