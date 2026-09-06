namespace Relay.Architecture.Tests;

internal static class ArchitectureProjects
{
    public static ArchitectureProject SharedKernel { get; } = new(
        Relay.SharedKernel.AssemblyReference.Assembly,
        "Relay.SharedKernel");

    public static ArchitectureProject BuildingBlocksInfrastructure { get; } = new(
        Infrastructure.AssemblyReference.Assembly,
        "Relay.Infrastructure");

    public static ArchitectureProject WorkflowsDomain { get; } = new(
        Workflows.Domain.AssemblyReference.Assembly,
        "Relay.Workflows.Domain");

    public static ArchitectureProject WorkflowsApplication { get; } = new(
        Workflows.Application.AssemblyReference.Assembly,
        "Relay.Workflows.Application");

    public static ArchitectureProject WorkflowsInfrastructure { get; } = new(
        Workflows.Infrastructure.AssemblyReference.Assembly,
        "Relay.Workflows.Infrastructure");

    public const string WebApiRootNamespace = "Hosts.WebApi";
}
