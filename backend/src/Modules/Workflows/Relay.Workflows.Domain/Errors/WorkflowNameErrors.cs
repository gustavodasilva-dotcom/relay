using Relay.SharedKernel;
using Relay.Workflows.Domain.ValueObjects;

namespace Relay.Workflows.Domain.Errors;

internal static class WorkflowNameErrors
{
    internal static readonly Error Required =
        new(
            "workflow_name.required",
            "The workflow name is required.");

    internal static readonly Error InvalidLength =
        new(
            "workflow_name.invalid_length",
            $"The workflow name must not exceed {WorkflowName.MaxLength} characters.");
}
