using System.Text.Json;

namespace Relay.Workflows.Features.IntegrationTests.Assertions;

internal static class JsonElementAssertions
{
    public static void AssertProperty(
        this JsonElement element,
        string propertyName)
    {
        Assert.True(
            element.TryGetProperty(propertyName, out _),
            $"Expected JSON property \"{propertyName}\" was not found.");
    }

    public static void AssertProperty(
        this JsonElement element,
        string propertyName,
        out JsonElement value)
    {
        Assert.True(
            element.TryGetProperty(propertyName, out value),
            $"Expected JSON property \"{propertyName}\" was not found.");
    }

    public static void AssertProperty(
        this JsonElement element,
        string propertyName,
        JsonValueKind expectedKind)
    {
        var exists = element.TryGetProperty(propertyName, out var property);

        Assert.True(
            exists,
            $"Expected JSON property \"{propertyName}\" was not found.");

        Assert.Equal(expectedKind, property.ValueKind);
    }
}
