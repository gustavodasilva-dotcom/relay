using System.Text.Json.Serialization;
using Relay.SharedKernel;
using Relay.Workflows.Domain.Errors;

namespace Relay.Workflows.Domain.ValueObjects;

public sealed class WorkflowName : ValueObject
{
    public const int MaxLength = 200;

    [JsonConstructor]
    private WorkflowName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public static Result<WorkflowName> Create(string? name)
    {
        var value = name?.Trim();

        if (string.IsNullOrWhiteSpace(value))
        {
            return WorkflowNameErrors.Required;
        }

        if (value.Length > MaxLength)
        {
            return WorkflowNameErrors.InvalidLength;
        }

        return new WorkflowName(value);
    }
}
