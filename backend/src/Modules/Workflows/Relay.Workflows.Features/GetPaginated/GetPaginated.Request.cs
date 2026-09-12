using Microsoft.AspNetCore.Mvc;
using Relay.Api.Contracts;

namespace Relay.Workflows.Features.GetPaginated;

public sealed class GetPaginatedRequest(
    [FromQuery] int page = 1,
    [FromQuery(Name = "page_size")]
    int pageSize = PaginatedRequest.DefaultPageSize)
    : PaginatedRequest(page, pageSize);
