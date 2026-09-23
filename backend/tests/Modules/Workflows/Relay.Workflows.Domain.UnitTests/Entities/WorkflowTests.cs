using Relay.Workflows.Domain.Entities;
using Relay.Workflows.Domain.Errors;
using Relay.Workflows.Domain.ValueObjects;

namespace Relay.Workflows.Domain.UnitTests.Entities;

public sealed class WorkflowTests
{
    private const string DefaultWorkflowName = "John Doe";
    private const string CharlieBrownName = "Charlie Brown";

    private static readonly DateTimeOffset TestDate =
        new(2026, 9, 4, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Constructor_initializes_workflow_with_expected_defaults()
    {
        // Arrange & Act
        var workflow = CreateWorkflow();
        var expectedName = WorkflowName.Create(DefaultWorkflowName).Value!;

        // Assert
        Assert.NotEqual(Guid.Empty, workflow.Id);
        Assert.Equal(expectedName, workflow.Name);
        Assert.True(workflow.Active);
        Assert.False(workflow.Deleted);
        Assert.Equal(TestDate, workflow.CreatedAt);
        Assert.Null(workflow.UpdatedAt);
        Assert.Null(workflow.DeletedAt);
    }

    [Theory]
    [InlineData(true, CharlieBrownName)]
    [InlineData(false, "Snoopy")]
    public void Update_returns_success_and_updates_workflow_state(
        bool active,
        string nameValue)
    {
        // Arrange
        var workflow = CreateWorkflow();
        var workflowName = WorkflowName.Create(nameValue).Value!;

        // Act
        var result = workflow.Update(workflowName, active, TestDate);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(workflowName, workflow.Name);
        Assert.Equal(active, workflow.Active);
        Assert.Equal(TestDate, workflow.UpdatedAt);
    }

    [Fact]
    public void Update_returns_deleted_error_when_workflow_state_is_deleted()
    {
        // Arrange
        var workflow = CreateWorkflow();
        workflow.Delete(TestDate);

        var workflowName = WorkflowName.Create(CharlieBrownName).Value!;

        // Act
        var result = workflow.Update(workflowName, active: true, TestDate);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(WorkflowErrors.Deleted, result.Error);
    }

    [Fact]
    public void Delete_returns_success_and_logically_deletes_workflow()
    {
        // Arrange
        var workflow = CreateWorkflow();

        // Act
        var result = workflow.Delete(TestDate);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(workflow.Deleted);
        Assert.Equal(TestDate, workflow.DeletedAt);
    }

    [Fact]
    public void Delete_returns_deleted_error_when_workflow_state_is_deleted()
    {
        // Arrange
        var workflow = CreateWorkflow();
        workflow.Delete(TestDate);

        // Act
        var result = workflow.Delete(TestDate);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(WorkflowErrors.Deleted, result.Error);
    }

    private static Workflow CreateWorkflow() =>
        new(WorkflowName.Create(DefaultWorkflowName).Value!, TestDate);
}
