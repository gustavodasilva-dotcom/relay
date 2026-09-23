using System.Net;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Json;
using Relay.Workflows.Domain.ValueObjects;
using Relay.Workflows.Features.Create;
using Relay.Workflows.Features.IntegrationTests.Assertions;
using Relay.Workflows.Features.IntegrationTests.Extensions;
using Relay.Workflows.Features.Shared;

namespace Relay.Workflows.Features.IntegrationTests.Create;

public sealed class CreateEndpointTests
{
    private const string RequestUri = "api/v1/workflows";

    [Fact]
    public async Task Post_returns_created_when_request_is_valid()
    {
        // Arrange
        await using var application =
            await IntegrationTestWebApplication.CreateAsync();

        var request = new CreateRequest { Name = "Test" };

        // Act
        var response = await application
            .Client.PostAsJsonAsync(RequestUri, request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var workflowResponse = await response
            .ReadRequiredFromJsonAsync<WorkflowResponse>();

        Assert.NotEqual(Guid.Empty, workflowResponse.Id);
        Assert.Equal(request.Name, workflowResponse.Name);
        Assert.True(workflowResponse.Active);
        Assert.NotEqual(default, workflowResponse.CreatedAt);
        Assert.Null(workflowResponse.UpdatedAt);

        Assert.Equal(
            $"workflows/{workflowResponse.Id}",
            response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task Post_returns_invalid_length_problem_when_name_exceeds_max_length()
    {
        // Arrange
        await using var application =
            await IntegrationTestWebApplication.CreateAsync();

        var request = new CreateRequest
        {
            Name = new string('a', WorkflowName.MaxLength + 1)
        };

        // Act
        var response = await application
            .Client.PostAsJsonAsync(RequestUri, request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        await AssertValidationProblemAsync(
            response,
            "workflow_name.invalid_length",
            $"The workflow name must not exceed {WorkflowName.MaxLength} characters.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Post_returns_required_problem_when_name_is_blank(string name)
    {
        // Arrange
        await using var application =
            await IntegrationTestWebApplication.CreateAsync();

        var request = new CreateRequest { Name = name };

        // Act
        var response = await application
            .Client.PostAsJsonAsync(RequestUri, request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        await AssertValidationProblemAsync(
            response,
            "workflow_name.required",
            "The workflow name is required.");
    }

    [Fact]
    public async Task Post_returns_bad_request_when_request_has_no_body()
    {
        // Arrange
        await using var application =
            await IntegrationTestWebApplication.CreateAsync();

        // Act
        var response = await application.Client.PostAsync(RequestUri, content: null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static async Task AssertValidationProblemAsync(
        HttpResponseMessage response,
        string expectedCode,
        string expectedDetail)
    {
        Assert.Equal(
            MediaTypeNames.Application.ProblemJson,
            response.Content.Headers.ContentType?.MediaType);

        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStreamAsync());

        var root = document.RootElement;
        root.AssertProperty("type");
        root.AssertProperty("title", out var titleElement);
        root.AssertProperty("status", out var statusElement);
        root.AssertProperty("detail", out var detailElement);
        root.AssertProperty("code", out var codeElement);

        Assert.Equal("Validation failed", titleElement.GetString());
        Assert.Equal((int)HttpStatusCode.BadRequest, statusElement.GetInt32());
        Assert.Equal(expectedDetail, detailElement.GetString());
        Assert.Equal(expectedCode, codeElement.GetString());
    }
}
