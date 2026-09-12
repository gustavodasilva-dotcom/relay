using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Relay.Api.Contracts;
using Relay.Workflows.Domain.Entities;
using Relay.Workflows.Domain.ValueObjects;
using Relay.Workflows.Features.GetPaginated;

namespace Relay.Workflows.Features.IntegrationTests.GetPaginated;

public sealed class GetPaginatedTests
{
    private const string Endpoint = "api/v1/workflows";

    [Fact]
    public async Task Get_returns_an_empty_page_when_there_are_no_workflows()
    {
        // Arrange
        await using var application =
            await IntegrationTestWebApplication.CreateAsync();

        // Act
        var response = await application.Client.GetAsync(Endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await ReadPageAsync(response);

        Assert.Empty(content.Items);
        Assert.Equal(1, content.Page);
        Assert.Equal(10, content.PageSize);
        Assert.Equal(0, content.TotalCount);
        Assert.Equal(0, content.TotalPages);
    }

    [Fact]
    public async Task Get_uses_default_pagination_and_orders_workflows_by_id()
    {
        // Arrange
        await using var application =
            await IntegrationTestWebApplication.CreateAsync();

        var workflows = Enumerable.Range(1, 12)
            .Select(CreateWorkflow)
            .ToArray();

        await application.SeedAsync(workflows);

        var expectedWorkflows = workflows
            .OrderBy(workflow => workflow.Id)
            .Take(10)
            .ToArray();

        // Act
        var response = await application.Client.GetAsync(Endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await ReadPageAsync(response);

        Assert.Equal(1, content.Page);
        Assert.Equal(10, content.PageSize);
        Assert.Equal(12, content.TotalCount);
        Assert.Equal(2, content.TotalPages);
        AssertResponses(content.Items, expectedWorkflows);
    }

    [Theory]
    [InlineData(2, 2, 2)]
    [InlineData(3, 2, 1)]
    [InlineData(4, 2, 0)]
    public async Task Get_returns_the_requested_page(
        int page,
        int pageSize,
        int expectedItemCount)
    {
        // Arrange
        await using var application =
            await IntegrationTestWebApplication.CreateAsync();

        var workflows = Enumerable.Range(1, 5)
            .Select(CreateWorkflow)
            .ToArray();

        await application.SeedAsync(workflows);

        var expectedWorkflows = workflows
            .OrderBy(workflow => workflow.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArray();

        // Act
        var response = await application.Client.GetAsync(
            $"{Endpoint}?page={page}&page_size={pageSize}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await ReadPageAsync(response);

        Assert.Equal(page, content.Page);
        Assert.Equal(pageSize, content.PageSize);
        Assert.Equal(5, content.TotalCount);
        Assert.Equal(3, content.TotalPages);
        Assert.Equal(expectedItemCount, content.Items.Count);
        AssertResponses(content.Items, expectedWorkflows);
    }

    [Fact]
    public async Task Get_maps_all_workflow_fields()
    {
        // Arrange
        await using var application =
            await IntegrationTestWebApplication.CreateAsync();

        var workflow = CreateWorkflow(1);
        var updatedAt = new DateTimeOffset(2026, 2, 1, 12, 0, 0, TimeSpan.Zero);
        var updatedName = WorkflowName.Create("Updated workflow").Value!;

        workflow.Update(updatedName, active: false, updatedAt: updatedAt);

        await application.SeedAsync(workflow);

        // Act
        var response = await application.Client.GetAsync(Endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await ReadPageAsync(response);
        var item = Assert.Single(content.Items);

        AssertResponse(item, workflow);
    }

    [Fact]
    public async Task Get_uses_the_expected_json_property_names()
    {
        // Arrange
        await using var application =
            await IntegrationTestWebApplication.CreateAsync();

        await application.SeedAsync(CreateWorkflow(1));

        // Act
        var response = await application.Client.GetAsync(Endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStreamAsync());

        var root = document.RootElement;
        Assert.True(root.TryGetProperty("items", out var items));
        Assert.True(root.TryGetProperty("page", out _));
        Assert.True(root.TryGetProperty("page_size", out _));
        Assert.True(root.TryGetProperty("total_count", out _));
        Assert.True(root.TryGetProperty("total_pages", out _));

        var item = items.EnumerateArray().Single();
        Assert.True(item.TryGetProperty("id", out _));
        Assert.True(item.TryGetProperty("name", out _));
        Assert.True(item.TryGetProperty("active", out _));
        Assert.True(item.TryGetProperty("created_at", out _));
        Assert.True(item.TryGetProperty("updated_at", out _));
    }

    [Theory]
    [InlineData("page=invalid")]
    [InlineData("page_size=invalid")]
    [InlineData("page=2147483648")]
    [InlineData("page_size=2147483648")]
    public async Task Get_returns_bad_request_when_query_parameters_are_invalid(
        string query)
    {
        // Arrange
        await using var application =
            await IntegrationTestWebApplication.CreateAsync();

        // Act
        var response = await application.Client.GetAsync($"{Endpoint}?{query}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("page=0")]
    [InlineData("page_size=101")]
    [InlineData("page=2147483647&page_size=2")]
    public async Task Get_returns_bad_request_when_pagination_validation_fails(
        string query)
    {
        // Arrange
        await using var application =
            await IntegrationTestWebApplication.CreateAsync();

        // Act
        var response = await application.Client.GetAsync($"{Endpoint}?{query}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_returns_method_not_allowed()
    {
        // Arrange
        await using var application =
            await IntegrationTestWebApplication.CreateAsync();

        // Act
        var response = await application.Client.PostAsync(Endpoint, content: null);

        // Assert
        Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
    }

    private static Workflow CreateWorkflow(int index)
    {
        var name = WorkflowName.Create($"Workflow {index}").Value!;
        var createdAt = new DateTimeOffset(2026, 1, index, 12, 0, 0, TimeSpan.Zero);

        return new Workflow(name, createdAt);
    }

    private static void AssertResponse(
        GetPaginatedResponse response,
        Workflow workflow)
    {
        Assert.Equal(workflow.Id, response.Id);
        Assert.Equal(workflow.Name.Value, response.Name);
        Assert.Equal(workflow.Active, response.Active);
        Assert.Equal(workflow.CreatedAt, response.CreatedAt);
        Assert.Equal(workflow.UpdatedAt, response.UpdatedAt);
    }

    private static void AssertResponses(
        IReadOnlyList<GetPaginatedResponse> responses,
        IReadOnlyList<Workflow> workflows)
    {
        Assert.Equal(workflows.Count, responses.Count);

        for (var index = 0; index < workflows.Count; index++)
        {
            AssertResponse(responses[index], workflows[index]);
        }
    }

    private static async Task<PaginatedResponse<GetPaginatedResponse>> ReadPageAsync(
        HttpResponseMessage response)
    {
        var content = await response.Content
            .ReadFromJsonAsync<PaginatedResponse<GetPaginatedResponse>>();

        return Assert.IsType<PaginatedResponse<GetPaginatedResponse>>(content);
    }
}
