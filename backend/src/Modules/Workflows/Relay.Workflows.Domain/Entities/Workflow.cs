using Relay.SharedKernel;
using Relay.Workflows.Domain.Errors;
using Relay.Workflows.Domain.ValueObjects;

namespace Relay.Workflows.Domain.Entities;

public sealed class Workflow : IAuditable
{
    public Workflow(WorkflowName name, DateTimeOffset createdAt)
    {
        Id = Guid.NewGuid();
        Name = name;
        Active = true;
        CreatedAt = createdAt;
    }

    private Workflow() { }

    public Guid Id { get; private set; }

    public WorkflowName Name { get; private set; } = null!;

    public bool Active { get; private set; }

    public bool Deleted { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public DateTimeOffset? DeletedAt { get; private set; }

    public Result Update(WorkflowName name, bool active, DateTimeOffset updatedAt)
    {
        if (Deleted)
        {
            return WorkflowErros.Deleted;
        }

        Name = name;
        Active = active;
        UpdatedAt = updatedAt;

        return Result.Success();
    }

    public Result Delete(DateTimeOffset deletedAt)
    {
        if (Deleted)
        {
            return WorkflowErros.Deleted;
        }

        Deleted = true;
        DeletedAt = deletedAt;

        return Result.Success();
    }
}
