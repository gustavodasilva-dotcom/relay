using Relay.SharedKernel;
using Relay.Workflows.Domain.Errors;
using Relay.Workflows.Domain.ValueObjects;

namespace Relay.Workflows.Domain.UnitTests.ValueObjects;

public sealed class WorkflowNameTests
{
    private const string JohnDoeName = "John Doe";

    [Fact]
    public void Create_returns_success_when_name_is_valid()
    {
        // Arrange & Act
        var result = WorkflowName.Create(JohnDoeName);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(JohnDoeName, result.Value!.Value);
    }

    [Fact]
    public void Create_trims_name_before_creating_workflow_name()
    {
        // Arrange & Act
        var result = WorkflowName.Create("   John Doe   ");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(JohnDoeName, result.Value!.Value);
    }

    [Fact]
    public void Create_returns_success_when_name_has_maximum_length()
    {
        // Arrange
        var name = new string('a', WorkflowName.MaxLength);

        // Act
        var result = WorkflowName.Create(name);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(name, result.Value!.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Create_returns_required_error_when_name_is_null_or_whitespace(string? value)
    {
        // Arrange & Act
        var result = WorkflowName.Create(value);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(WorkflowNameErrors.Required, result.Error);
    }

    [Fact]
    public void Create_returns_invalid_length_error_when_name_exceeds_maximum_length()
    {
        // Arrange
        var name = new string('a', WorkflowName.MaxLength + 1);

        // Act
        var result = WorkflowName.Create(name);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(WorkflowNameErrors.InvalidLength, result.Error);
    }
}
