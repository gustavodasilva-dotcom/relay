using Relay.SharedKernel;

namespace Relay.Workflows.Features.Errors;

internal static class WorkflowErrors
{
    public static Error NotFound(Guid id) =>
        new(
            "workflows.not_found",
            $"The workflow with ID \"{id}\" was not found.",
            ErrorType.NotFound);
}
