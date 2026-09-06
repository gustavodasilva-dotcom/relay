using System.Reflection;

namespace Relay.SharedKernel;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
