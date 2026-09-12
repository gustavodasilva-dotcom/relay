using Microsoft.EntityFrameworkCore;
using Relay.Infrastructure.Abstractions;
using Relay.Workflows.Domain.Entities;

namespace Relay.Workflows.Infrastructure.Data;

public sealed class WorkflowsDbContext(
    DbContextOptions<WorkflowsDbContext> options)
    : AuditingDbContext<WorkflowsDbContext>(options)
{
    protected override string Schema => "workflows";

    public DbSet<Workflow> Workflows => Set<Workflow>();

    protected override void ConfigureDomainModel(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            AssemblyReference.Assembly);
    }
}
