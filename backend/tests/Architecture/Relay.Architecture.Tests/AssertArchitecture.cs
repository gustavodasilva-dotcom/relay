using NetArchTest.Rules;

namespace Relay.Architecture.Tests;

internal static class AssertArchitecture
{
    public static void Success(TestResult result)
    {
        Assert.True(
            result.IsSuccessful,
            $"Failing types: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }
}
