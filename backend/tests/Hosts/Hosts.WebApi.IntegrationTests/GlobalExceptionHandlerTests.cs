using System.Net;
using System.Net.Http.Json;
using Hosts.WebApi.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Hosts.WebApi.IntegrationTests;

public sealed class GlobalExceptionHandlerTests
{
    [Fact]
    public async Task ExceptionHandler_WhenEndpointThrows_ReturnsInternalServerErrorProblemDetails()
    {
        // Arrange
        const string ExceptionMessage = "An unexpected failure occurred.";

        var builder = WebApplication.CreateBuilder();

        builder.WebHost.UseTestServer();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        await using var application = builder.Build();

        application.UseExceptionHandler();
        application.MapGet("/throws", (HttpContext _) =>
            Task.FromException(new InvalidOperationException(ExceptionMessage)));

        await application.StartAsync();

        using var client = application.GetTestClient();

        // Act
        using var response = await client.GetAsync("/throws");

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal(
            "application/problem+json",
            response.Content.Headers.ContentType?.MediaType);

        var problemDetails =
            await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.NotNull(problemDetails);
        Assert.Equal(StatusCodes.Status500InternalServerError, problemDetails.Status);
        Assert.Equal("An error occurred", problemDetails.Title);
        Assert.Equal(ExceptionMessage, problemDetails.Detail);
    }
}
