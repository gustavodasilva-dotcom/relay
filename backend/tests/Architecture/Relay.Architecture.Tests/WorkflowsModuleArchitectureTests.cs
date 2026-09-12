using System.Reflection;
using NetArchTest.Rules;

namespace Relay.Architecture.Tests;

public sealed class WorkflowsModuleArchitectureTests
{
    private const string DomainValueObjectsNamespace = "Relay.Workflows.Domain.ValueObjects";
    private const string DomainEntitiesNamespace = "Relay.Workflows.Domain.Entities";

    [Fact]
    public void Domain_layer_should_not_have_dependency_on_other_module_projects()
    {
        // Arrange
        var forbiddenNamespaces = new string[]
        {
            ArchitectureProjects.ApiContracts.RootNamespace,
            ArchitectureProjects.WorkflowsFeatures.RootNamespace,
            ArchitectureProjects.WorkflowsInfrastructure.RootNamespace
        };

        // Act
        var result = Types
            .InAssembly(ArchitectureProjects.WorkflowsDomain.Assembly)
            .Should()
            .NotHaveDependencyOnAny(forbiddenNamespaces)
            .GetResult();

        // Assert
        AssertArchitecture.Success(result);
    }

    [Fact]
    public void Infrastructure_layer_should_not_have_dependency_on_Features_or_Routing()
    {
        // Arrange
        var forbiddenNamespaces = new[]
        {
            ArchitectureProjects.ApiContracts.RootNamespace,
            ArchitectureProjects.Routing.RootNamespace,
            ArchitectureProjects.WorkflowsFeatures.RootNamespace
        };

        // Act
        var result = Types
            .InAssembly(ArchitectureProjects.WorkflowsInfrastructure.Assembly)
            .Should()
            .NotHaveDependencyOnAny(forbiddenNamespaces)
            .GetResult();

        // Assert
        AssertArchitecture.Success(result);
    }

    [Fact]
    public void Domain_ValueObjects_should_be_sealed()
    {
        // Act
        var result = Types
            .InAssembly(ArchitectureProjects.WorkflowsDomain.Assembly)
            .That()
            .ResideInNamespace(DomainValueObjectsNamespace)
            .Should()
            .BeSealed()
            .GetResult();

        // Assert
        AssertArchitecture.Success(result);
    }

    [Fact]
    public void Domain_ValueObjects_should_inherit_from_SharedKernel_ValueObject()
    {
        // Act
        var result = Types
            .InAssembly(ArchitectureProjects.WorkflowsDomain.Assembly)
            .That()
            .ResideInNamespace(DomainValueObjectsNamespace)
            .Should()
            .Inherit(typeof(SharedKernel.ValueObject))
            .GetResult();

        // Assert
        AssertArchitecture.Success(result);
    }

    [Fact]
    public void Domain_ValueObjects_should_not_have_public_constructors()
    {
        // Arrange
        var valueObjectTypes = Types
            .InAssembly(ArchitectureProjects.WorkflowsDomain.Assembly)
            .That()
            .ResideInNamespace(DomainValueObjectsNamespace)
            .GetTypes();

        // Act
        Assert.NotEmpty(valueObjectTypes);

        var failingTypes = new List<Type>();

        foreach (var valueObjectType in valueObjectTypes)
        {
            var constructors = valueObjectType
                .GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            if (constructors.Length > 0)
            {
                failingTypes.Add(valueObjectType);
            }
        }

        // Assert
        Assert.Empty(failingTypes);
    }

    [Fact]
    public void Domain_Entities_should_be_sealed()
    {
        // Act
        var result = Types
            .InAssembly(ArchitectureProjects.WorkflowsDomain.Assembly)
            .That()
            .ResideInNamespace(DomainEntitiesNamespace)
            .Should()
            .BeSealed()
            .GetResult();

        // Assert
        AssertArchitecture.Success(result);
    }

    [Fact]
    public void Domain_Entities_should_implement_interface_SharedKernel_IAuditable()
    {
        // Act
        var result = Types
            .InAssembly(ArchitectureProjects.WorkflowsDomain.Assembly)
            .That()
            .ResideInNamespace(DomainEntitiesNamespace)
            .Should()
            .ImplementInterface(typeof(SharedKernel.IAuditable))
            .GetResult();

        // Assert
        AssertArchitecture.Success(result);
    }

    [Fact]
    public void Domain_Entities_should_have_public_constructors()
    {
        // Arrange
        var entityTypes = Types
            .InAssembly(ArchitectureProjects.WorkflowsDomain.Assembly)
            .That()
            .ResideInNamespace(DomainEntitiesNamespace)
            .GetTypes();

        // Act
        Assert.NotEmpty(entityTypes);

        var failingTypes = new List<Type>();

        foreach (var entityType in entityTypes)
        {
            var constructors = entityType
                .GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            if (constructors.Length == 0)
            {
                failingTypes.Add(entityType);
            }
        }

        // Assert
        Assert.Empty(failingTypes);
    }
}
