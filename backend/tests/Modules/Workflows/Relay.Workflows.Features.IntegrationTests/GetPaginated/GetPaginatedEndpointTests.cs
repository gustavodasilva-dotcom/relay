using System.Net;
using System.Text.Json;
using Relay.Api.Contracts;
using Relay.Workflows.Domain.ValueObjects;
using Relay.Workflows.Features.IntegrationTests.Assertions;
using Relay.Workflows.Features.IntegrationTests.Extensions;
using Relay.Workflows.Features.IntegrationTests.Fixtures;
using Relay.Workflows.Features.Shared;

namespace Relay.Workflows.Features.IntegrationTests.GetPaginated;

public sealed class GetPaginatedEndpointTests
{
    private const string RequestUri = "api/v1/workflows";

    [Fact]
    public async Task Get_returns_an_empty_page_when_there_are_no_workflows()
    {
        // Arrange
        await using var application =
            await IntegrationTestWebApplication.CreateAsync();

        // Act
        var response = await application.Client.GetAsync(RequestUri);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var pageResponse = await response
            .ReadRequiredFromJsonAsync<PaginatedResponse<WorkflowResponse>>();

        Assert.Empty(pageResponse.Items);
        Assert.Equal(PaginatedRequest.DefaultPage, pageResponse.Page);
        Assert.Equal(PaginatedRequest.DefaultPageSize, pageResponse.PageSize);
        Assert.Equal(0, pageResponse.TotalCount);
        Assert.Equal(0, pageResponse.TotalPages);
    }

    [Fact]
    public async Task Get_uses_default_pagination_and_orders_workflows_by_id()
    {
        // Arrange
        await using var application =
            await IntegrationTestWebApplication.CreateAsync();

        var workflows = Enumerable.Range(1, 12)
            .Select(WorkflowFactory.Create)
            .ToArray();

        await application.SeedAsync(workflows);

        var expectedWorkflows = workflows
            .OrderBy(workflow => workflow.Id)
            .Take(10)
            .ToArray();

        // Act
        var response = await application.Client.GetAsync(RequestUri);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var pageResponse = await response
            .ReadRequiredFromJsonAsync<PaginatedResponse<WorkflowResponse>>();

        Assert.Equal(PaginatedRequest.DefaultPage, pageResponse.Page);
        Assert.Equal(PaginatedRequest.DefaultPageSize, pageResponse.PageSize);
        Assert.Equal(12, pageResponse.TotalCount);
        Assert.Equal(2, pageResponse.TotalPages);
        pageResponse.Items.ShouldMatch(expectedWorkflows);
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
            .Select(WorkflowFactory.Create)
            .ToArray();

        await application.SeedAsync(workflows);

        var expectedWorkflows = workflows
            .OrderBy(workflow => workflow.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArray();

        // Act
        var response = await application.Client.GetAsync(
            $"{RequestUri}?page={page}&page_size={pageSize}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var pageResponse = await response
            .ReadRequiredFromJsonAsync<PaginatedResponse<WorkflowResponse>>();

        Assert.Equal(page, pageResponse.Page);
        Assert.Equal(pageSize, pageResponse.PageSize);
        Assert.Equal(5, pageResponse.TotalCount);
        Assert.Equal(3, pageResponse.TotalPages);
        Assert.Equal(expectedItemCount, pageResponse.Items.Count);
        pageResponse.Items.ShouldMatch(expectedWorkflows);
    }

    [Fact]
    public async Task Get_maps_all_workflow_fields()
    {
        // Arrange
        await using var application =
            await IntegrationTestWebApplication.CreateAsync();

        var workflow = WorkflowFactory.Create();
        var updatedAt = new DateTimeOffset(2026, 2, 1, 12, 0, 0, TimeSpan.Zero);
        var updatedName = WorkflowName.Create("Updated workflow").Value!;

        workflow.Update(updatedName, active: false, updatedAt: updatedAt);

        await application.SeedAsync(workflow);

        // Act
        var response = await application.Client.GetAsync(RequestUri);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var pageResponse = await response
            .ReadRequiredFromJsonAsync<PaginatedResponse<WorkflowResponse>>();

        var item = Assert.Single(pageResponse.Items);

        item.ShouldMatch(workflow);
    }

    [Fact]
    public async Task Get_uses_the_expected_json_property_names()
    {
        // Arrange
        await using var application =
            await IntegrationTestWebApplication.CreateAsync();

        await application.SeedAsync(WorkflowFactory.Create());

        // Act
        var response = await application.Client.GetAsync(RequestUri);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStreamAsync());

        var root = document.RootElement;
        Assert.True(root.TryGetProperty("items", out var items));
        root.AssertProperty("page");
        root.AssertProperty("page_size");
        root.AssertProperty("total_count");
        root.AssertProperty("total_pages");

        var item = items.EnumerateArray().Single();
        item.AssertProperty("id");
        item.AssertProperty("name");
        item.AssertProperty("active");
        item.AssertProperty("created_at");
        item.AssertProperty("updated_at");
    }

    [Fact]
    public async Task Get_uses_the_expected_json_property_value_kinds()
    {
        // Arrange
        await using var application =
            await IntegrationTestWebApplication.CreateAsync();

        await application.SeedAsync(WorkflowFactory.Create());

        // Act
        var response = await application.Client.GetAsync(RequestUri);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var document = JsonDocument.Parse(
            await response.Content.ReadAsStreamAsync());

        var root = document.RootElement;
        root.AssertProperty("items", JsonValueKind.Array);
        root.AssertProperty("page", JsonValueKind.Number);
        root.AssertProperty("page_size", JsonValueKind.Number);
        root.AssertProperty("total_count", JsonValueKind.Number);
        root.AssertProperty("total_pages", JsonValueKind.Number);

        root.TryGetProperty("items", out var items);

        var item = items.EnumerateArray().Single();
        item.AssertProperty("id", JsonValueKind.String);
        item.AssertProperty("name", JsonValueKind.String);
        item.AssertProperty("active", JsonValueKind.True);
        item.AssertProperty("created_at", JsonValueKind.String);
        item.AssertProperty("updated_at", JsonValueKind.Null);
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
        var response = await application.Client.GetAsync($"{RequestUri}?{query}");

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
        var response = await application.Client.GetAsync($"{RequestUri}?{query}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
