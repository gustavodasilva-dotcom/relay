using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Relay.Infrastructure.Abstractions;
using Relay.Infrastructure.Models;
using Relay.SharedKernel;

namespace Relay.Infrastructure.Interceptors;

public sealed class AuditLoggingInterceptor(
    TimeProvider timeProvider,
    IAuditActorProvider actorProvider) : SaveChangesInterceptor
{
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly IAuditActorProvider _actorProvider = actorProvider;

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;

        if (context is null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var auditEntries = CreateAuditEntries(context);

        context.Set<AuditLogEntry>().AddRange(auditEntries);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        var context = eventData.Context;

        if (context is null)
        {
            return base.SavingChanges(eventData, result);
        }

        var auditEntries = CreateAuditEntries(context);

        context.Set<AuditLogEntry>().AddRange(auditEntries);

        return base.SavingChanges(eventData, result);
    }

    private List<AuditLogEntry> CreateAuditEntries(DbContext context)
    {
        var entries = new List<AuditLogEntry>();

        context.ChangeTracker.DetectChanges();

        foreach (var entry in context.ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State is EntityState.Detached or EntityState.Unchanged)
            {
                continue;
            }

            var auditEntry = new AuditLogEntry
            {
                Id = Guid.NewGuid(),
                EntityName = entry.Entity.GetType().Name,
                EntityId = GetPrimaryKey(entry),
                UserId = _actorProvider.GetActorId(),
                Timestamp = _timeProvider.GetUtcNow(),
                Action = entry.State.ToString()
            };

            switch (entry.State)
            {
                case EntityState.Added:
                    auditEntry.NewValues = SerializeProperties(entry.Properties);
                    break;

                case EntityState.Modified:
                    auditEntry.OldValues = SerializeOldValues(entry);
                    auditEntry.NewValues = SerializeNewValues(entry);
                    auditEntry.AffectedColumns = GetModifiedColumns(entry);
                    break;

                case EntityState.Deleted:
                    auditEntry.OldValues = SerializeProperties(entry.Properties);
                    break;
            }

            entries.Add(auditEntry);
        }

        return entries;
    }

    private static string GetPrimaryKey(EntityEntry entry)
    {
        var keyParts = entry.Properties
            .Where(p => p.Metadata.IsPrimaryKey())
            .Select(p => p.CurrentValue?.ToString() ?? "null");

        return string.Join(", ", keyParts);
    }

    private static string SerializeProperties(
        IEnumerable<PropertyEntry> properties)
    {
        var dict = properties.ToDictionary(
            p => p.Metadata.Name,
            p => p.CurrentValue);

        return JsonSerializer.Serialize(dict);
    }

    private static string SerializeOldValues(EntityEntry entry)
    {
        var dict = entry.Properties
            .Where(p => p.IsModified)
            .ToDictionary(
                p => p.Metadata.Name,
                p => p.OriginalValue);

        return JsonSerializer.Serialize(dict);
    }

    private static string SerializeNewValues(EntityEntry entry)
    {
        var dict = entry.Properties
            .Where(p => p.IsModified)
            .ToDictionary(
                p => p.Metadata.Name,
                p => p.CurrentValue);

        return JsonSerializer.Serialize(dict);
    }

    private static string GetModifiedColumns(EntityEntry entry)
    {
        var columns = entry.Properties
            .Where(p => p.IsModified)
            .Select(p => p.Metadata.Name);

        return string.Join(", ", columns);
    }
}
