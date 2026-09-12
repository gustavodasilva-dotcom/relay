using Asp.Versioning;
using Hosts.WebApi.Impl;
using Relay.Infrastructure.Abstractions;
using Relay.Routing.Extensions;
using Relay.Workflows.Features;
using Relay.Workflows.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuditActorProvider, WebApiAuditActorProvider>();

builder.Services.AddSingleton(TimeProvider.System);

builder.Services
    .AddWorkflowsInfrastructure()
    .AddWorkflowsFeatures();

builder.Services.AddEndpoints(
    Relay.Workflows.Features.AssemblyReference.Assembly);

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"));
})
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'V";
        options.SubstituteApiVersionInUrl = true;
    })
    .AddOpenApi();

builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().WithDocumentPerVersion();
}

app.UseHttpsRedirection();

app.UseWorkflowsInfrastructure();

var apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

var group = app
    .MapGroup("api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersionSet);

app.MapEndpoints(group);

app.Run();
