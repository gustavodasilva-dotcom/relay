using Hosts.WebApi.Impl;
using Relay.Infrastructure.Abstractions;
using Relay.Workflows.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuditActorProvider, WebApiAuditActorProvider>();

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddInfrastructure();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
