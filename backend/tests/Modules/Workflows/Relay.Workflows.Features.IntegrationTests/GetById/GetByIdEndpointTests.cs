using System.Net;
using System.Text.Json;
using Relay.Workflows.Features.IntegrationTests.Assertions;
using Relay.Workflows.Features.IntegrationTests.Extensions;
using Relay.Workflows.Features.IntegrationTests.Fixtures;
using Relay.Workflows.Features.Shared;

namespace Relay.Workflows.Features.IntegrationTests.GetById;

public sealed class GetByIdEndpointTests
{
    [Fact]
    public async Task Get_returns_ok_with_existing_workflow()
    {
        // Arrange
        await using var application =
            await IntegrationTestWebApplication.CreateAsync();

        var workflow = WorkflowFactory.Create();

        await application.SeedAsync(workflow);

        var requestUri = GetRequestUri(workflow.Id);

        // Act
        var response = await application.Client.GetAsync(requestUri);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var workflowResponse = await response
            .ReadRequiredFromJsonAsync<WorkflowResponse>();

        workflowResponse.ShouldMatch(workflow);
    }

    [Fact]
    public async Task Get_returns_not_found_when_unknown_workflow_is_requested()
    {
        // Arrange
        await using var application =
            await IntegrationTestWebApplication.CreateAsync();

        var workflows = Enumerable.Range(1, 5)
            .Select(WorkflowFactory.Create)
            .ToArray();

        await application.SeedAsync(workflows);

        var requestUri = GetRequestUri(Guid.NewGuid());

        // Act
        var response = await application.Client.GetAsync(requestUri);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_returns_not_found_when_deleted_workflow_is_requested()
    {
        // Arrange
        await using var application =
            await IntegrationTestWebApplication.CreateAsync();

        var workflow = WorkflowFactory.Create();

        var deletedAt = new DateTimeOffset(2026, 1, 14, 12, 0, 0, TimeSpan.Zero);
        workflow.Delete(deletedAt);

        await application.SeedAsync(workflow);

        var requestUri = GetRequestUri(workflow.Id);

        // Act
        var response = await application.Client.GetAsync(requestUri);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_returns_not_found_when_id_is_not_a_valid_guid()
    {
        // Arrange
        const string Endpoint = "api/v1/workflows/123";

        await using var application =
            await IntegrationTestWebApplication.CreateAsync();

        // Act
        var response = await application.Client.GetAsync(Endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_returns_expected_json_property_names_when_workflow_exists()
    {
        // Arrange
        await using var application =
            await IntegrationTestWebApplication.CreateAsync();

        var workflow = WorkflowFactory.Create();

        await application.SeedAsync(workflow);

        var requestUri = GetRequestUri(workflow.Id);

        // Act
        var response = await application.Client.GetAsync(requestUri);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStreamAsync());

        var root = document.RootElement;
        root.AssertProperty("id");
        root.AssertProperty("name");
        root.AssertProperty("active");
        root.AssertProperty("created_at");
        root.AssertProperty("updated_at");
    }

    [Fact]
    public async Task Get_returns_expected_json_value_kinds_when_workflow_exists()
    {
        // Arrange
        await using var application =
            await IntegrationTestWebApplication.CreateAsync();

        var workflow = WorkflowFactory.Create();

        await application.SeedAsync(workflow);

        var requestUri = GetRequestUri(workflow.Id);

        // Act
        var response = await application.Client.GetAsync(requestUri);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStreamAsync());

        var root = document.RootElement;
        root.AssertProperty("id", JsonValueKind.String);
        root.AssertProperty("name", JsonValueKind.String);
        root.AssertProperty("active", JsonValueKind.True);
        root.AssertProperty("created_at", JsonValueKind.String);
        root.AssertProperty("updated_at", JsonValueKind.Null);
    }

    [Fact]
    public async Task Get_returns_expected_problem_details_when_workflow_is_not_found()
    {
        // Arrange
        await using var application =
            await IntegrationTestWebApplication.CreateAsync();

        var requestUri = GetRequestUri(Guid.NewGuid());

        // Act
        var response = await application.Client.GetAsync(requestUri);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStreamAsync());

        var root = document.RootElement;
        root.AssertProperty("type");
        root.AssertProperty("title");
        root.AssertProperty("status");
        root.AssertProperty("detail");
        root.AssertProperty("code");
    }

    private static string GetRequestUri(Guid id) =>
        $"api/v1/workflows/{id}";
}
