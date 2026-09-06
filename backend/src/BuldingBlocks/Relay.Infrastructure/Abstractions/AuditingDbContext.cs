using Microsoft.EntityFrameworkCore;
using Relay.Infrastructure.Configurations;
using Relay.Infrastructure.Models;

namespace Relay.Infrastructure.Abstractions;

public abstract class AuditingDbContext<TContext>(
    DbContextOptions<TContext> options)
    : DbContext(options)
    where TContext : DbContext
{
    protected abstract string Schema { get; }

    public DbSet<AuditLogEntry> AuditLogEntries =>
        Set<AuditLogEntry>();

    protected sealed override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(Schema);

        ConfigureDomainModel(modelBuilder);
        ConfigureAuditModel(modelBuilder);
    }

    protected abstract void ConfigureDomainModel(ModelBuilder modelBuilder);

    private static void ConfigureAuditModel(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AuditLogEntryConfiguration());
    }
}
