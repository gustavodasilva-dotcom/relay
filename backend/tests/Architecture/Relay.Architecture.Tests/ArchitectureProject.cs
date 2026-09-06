using System.Reflection;

namespace Relay.Architecture.Tests;

internal sealed record ArchitectureProject(
    Assembly Assembly,
    string RootNamespace);
