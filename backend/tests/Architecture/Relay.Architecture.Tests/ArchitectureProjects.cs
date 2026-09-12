namespace Relay.Architecture.Tests;

internal static class ArchitectureProjects
{
    public const string WebApiRootNamespace = "Hosts.WebApi";

    public static ArchitectureProject ApiContracts { get; } = new(
        typeof(Relay.Api.Contracts.PaginatedResponse<>).Assembly,
        "Relay.Api.Contracts");

    public static ArchitectureProject SharedKernel { get; } = new(
        Relay.SharedKernel.AssemblyReference.Assembly,
        "Relay.SharedKernel");

    public static ArchitectureProject BuildingBlocksInfrastructure { get; } = new(
        Infrastructure.AssemblyReference.Assembly,
        "Relay.Infrastructure");

    public static ArchitectureProject Routing { get; } = new(
        typeof(Relay.Routing.Abstractions.IEndpoint).Assembly,
        "Relay.Routing");

    public static ArchitectureProject WorkflowsDomain { get; } = new(
        Workflows.Domain.AssemblyReference.Assembly,
        "Relay.Workflows.Domain");

    public static ArchitectureProject WorkflowsFeatures { get; } = new(
        Workflows.Features.AssemblyReference.Assembly,
        "Relay.Workflows.Features");

    public static ArchitectureProject WorkflowsInfrastructure { get; } = new(
        Workflows.Infrastructure.AssemblyReference.Assembly,
        "Relay.Workflows.Infrastructure");

}
