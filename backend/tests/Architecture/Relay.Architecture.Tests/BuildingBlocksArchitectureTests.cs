using NetArchTest.Rules;

namespace Relay.Architecture.Tests;

public sealed class BuildingBlocksArchitectureTests
{
    [Fact]
    public void SharedKernel_should_not_have_dependency_on_other_projects()
    {
        // Arrange
        var forbiddenNamespaces = new[]
        {
            ArchitectureProjects.ApiContracts.RootNamespace,
            ArchitectureProjects.BuildingBlocksInfrastructure.RootNamespace,
            ArchitectureProjects.Routing.RootNamespace,
            ArchitectureProjects.WebApiRootNamespace,
            ArchitectureProjects.WorkflowsFeatures.RootNamespace,
            ArchitectureProjects.WorkflowsDomain.RootNamespace,
            ArchitectureProjects.WorkflowsInfrastructure.RootNamespace
        };

        // Act
        var result = Types
            .InAssembly(ArchitectureProjects.SharedKernel.Assembly)
            .Should()
            .NotHaveDependencyOnAny(forbiddenNamespaces)
            .GetResult();

        // Assert
        AssertArchitecture.Success(result);
    }

    [Fact]
    public void Infrastructure_should_not_depend_on_modules_or_hosts()
    {
        // Arrange
        var forbiddenNamespaces = new[]
        {
            ArchitectureProjects.ApiContracts.RootNamespace,
            ArchitectureProjects.Routing.RootNamespace,
            ArchitectureProjects.WebApiRootNamespace,
            ArchitectureProjects.WorkflowsFeatures.RootNamespace,
            ArchitectureProjects.WorkflowsDomain.RootNamespace,
            ArchitectureProjects.WorkflowsInfrastructure.RootNamespace
        };

        // Act
        var result = Types
            .InAssembly(
                ArchitectureProjects.BuildingBlocksInfrastructure.Assembly)
            .Should()
            .NotHaveDependencyOnAny(forbiddenNamespaces)
            .GetResult();

        // Assert
        AssertArchitecture.Success(result);
    }

    [Fact]
    public void Routing_should_not_depend_on_other_projects()
    {
        // Arrange
        var forbiddenNamespaces = new[]
        {
            ArchitectureProjects.ApiContracts.RootNamespace,
            ArchitectureProjects.BuildingBlocksInfrastructure.RootNamespace,
            ArchitectureProjects.SharedKernel.RootNamespace,
            ArchitectureProjects.WebApiRootNamespace,
            ArchitectureProjects.WorkflowsDomain.RootNamespace,
            ArchitectureProjects.WorkflowsFeatures.RootNamespace,
            ArchitectureProjects.WorkflowsInfrastructure.RootNamespace
        };

        // Act
        var result = Types
            .InAssembly(ArchitectureProjects.Routing.Assembly)
            .Should()
            .NotHaveDependencyOnAny(forbiddenNamespaces)
            .GetResult();

        // Assert
        AssertArchitecture.Success(result);
    }

    [Fact]
    public void ApiContracts_should_not_depend_on_other_projects()
    {
        // Arrange
        var forbiddenNamespaces = new[]
        {
            ArchitectureProjects.BuildingBlocksInfrastructure.RootNamespace,
            ArchitectureProjects.Routing.RootNamespace,
            ArchitectureProjects.SharedKernel.RootNamespace,
            ArchitectureProjects.WebApiRootNamespace,
            ArchitectureProjects.WorkflowsDomain.RootNamespace,
            ArchitectureProjects.WorkflowsFeatures.RootNamespace,
            ArchitectureProjects.WorkflowsInfrastructure.RootNamespace
        };

        // Act
        var result = Types
            .InAssembly(ArchitectureProjects.ApiContracts.Assembly)
            .Should()
            .NotHaveDependencyOnAny(forbiddenNamespaces)
            .GetResult();

        // Assert
        AssertArchitecture.Success(result);
    }
}
