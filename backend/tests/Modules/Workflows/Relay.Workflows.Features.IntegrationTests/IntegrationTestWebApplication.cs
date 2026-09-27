using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Relay.Routing.Extensions;
using Relay.Workflows.Domain.Entities;
using Relay.Workflows.Infrastructure.Data;

namespace Relay.Workflows.Features.IntegrationTests;

internal sealed class IntegrationTestWebApplication : IAsyncDisposable
{
    private readonly WebApplication _application;

    private IntegrationTestWebApplication(WebApplication application)
    {
        _application = application;
        Client = application.GetTestClient();
    }

    public HttpClient Client { get; }

    public static async Task<IntegrationTestWebApplication> CreateAsync()
    {
        var builder = WebApplication.CreateBuilder();
        var databaseName = Guid.NewGuid().ToString();

        builder.WebHost.UseTestServer();

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddDbContext<WorkflowsDbContext>(options =>
            options.UseInMemoryDatabase(databaseName));
        builder.Services.AddHybridCache();
        builder.Services.AddWorkflowsFeatures();
        builder.Services.AddEndpoints(AssemblyReference.Assembly);

        var application = builder.Build();
        var group = application.MapGroup("api/v1");

        application.MapEndpoints(group);

        await application.StartAsync();

        return new IntegrationTestWebApplication(application);
    }

    public async Task SeedAsync(params Workflow[] workflows)
    {
        await using var scope = _application.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WorkflowsDbContext>();

        dbContext.Workflows.AddRange(workflows);
        await dbContext.SaveChangesAsync();
    }

    public async ValueTask DisposeAsync()
    {
        Client.Dispose();
        await _application.DisposeAsync();
    }
}
