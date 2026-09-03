using Relay.SharedKernel;
using Relay.Workflows.Domain.Errors;

namespace Relay.Workflows.Domain.ValueObjects;

public sealed class WorkflowName : ValueObject
{
    private WorkflowName(string name)
    {
        Name = name;
    }

    public string Name { get; }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Name;
    }

    public static Result<WorkflowName> Create(string? name)
    {
        var value = name?.Trim();

        if (string.IsNullOrWhiteSpace(value))
        {
            return WorkflowNameErrors.Required;
        }

        if (value.Length > DomainConstraints.MaxTitleLength)
        {
            return WorkflowNameErrors.InvalidLength;
        }

        return new WorkflowName(value);
    }
}
