using System.Net.Http.Json;

namespace Relay.Workflows.Features.IntegrationTests.Extensions;

internal static class HttpResponseMessageExtensions
{
    public static async Task<T> ReadRequiredFromJsonAsync<T>(
        this HttpResponseMessage response)
    {
        var content = await response.Content.ReadFromJsonAsync<T>();

        return Assert.IsType<T>(content);
    }
}
