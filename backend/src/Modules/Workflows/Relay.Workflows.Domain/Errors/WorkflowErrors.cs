using Relay.SharedKernel;

namespace Relay.Workflows.Domain.Errors;

internal static class WorkflowErrors
{
    internal static readonly Error Deleted =
        new(
            "workflows.deleted",
            "The requested workflow has been deleted and cannot be modified.",
            ErrorType.Conflict);
}
