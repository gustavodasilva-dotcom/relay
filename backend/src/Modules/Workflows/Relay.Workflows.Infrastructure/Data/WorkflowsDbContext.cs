using Microsoft.EntityFrameworkCore;
using Relay.Infrastructure.Abstractions;

namespace Relay.Workflows.Infrastructure.Data;

internal sealed class WorkflowsDbContext(
    DbContextOptions<WorkflowsDbContext> options)
    : AuditingDbContext<WorkflowsDbContext>(options)
{
    protected override string Schema => "workflows";

    protected override void ConfigureDomainModel(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            AssemblyReference.Assembly);
    }
}
