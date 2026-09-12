using Microsoft.EntityFrameworkCore;
using Relay.Api.Contracts;
using Relay.Workflows.Infrastructure.Data;

namespace Relay.Workflows.Features.GetPaginated;

internal sealed class GetPaginatedHandler(WorkflowsDbContext dbContext)
{
    public async Task<PaginatedResponse<GetPaginatedResponse>> Handle(
        GetPaginatedRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Workflows
            .AsNoTracking()
            .OrderBy(workflow => workflow.Id);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(workflow => new GetPaginatedResponse
            {
                Id = workflow.Id,
                Name = workflow.Name.Value,
                Active = workflow.Active,
                CreatedAt = workflow.CreatedAt,
                UpdatedAt = workflow.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return new(items, request.Page, request.PageSize, totalCount);
    }
}
