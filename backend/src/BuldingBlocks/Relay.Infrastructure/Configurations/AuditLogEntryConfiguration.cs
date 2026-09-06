using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Relay.Infrastructure.Models;

namespace Relay.Infrastructure.Configurations;

internal sealed class AuditLogEntryConfiguration : IEntityTypeConfiguration<AuditLogEntry>
{
    public void Configure(EntityTypeBuilder<AuditLogEntry> builder)
    {
        builder.ToTable("AuditLogEntries");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EntityName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.EntityId)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Action)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasMaxLength(200);

        builder.Property(x => x.OldValues)
            .HasColumnType("text");

        builder.Property(x => x.NewValues)
            .HasColumnType("text");

        builder.Property(x => x.AffectedColumns)
            .HasColumnType("text");

        builder.HasIndex(x => x.EntityName);

        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => x.Timestamp);

        builder.HasIndex(x => new { x.EntityName, x.EntityId });
    }
}
