using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Relay.Workflows.Domain.Entities;
using Relay.Workflows.Domain.ValueObjects;

namespace Relay.Workflows.Infrastructure.Data.Configurations;

internal sealed class WorkflowConfiguration : IEntityTypeConfiguration<Workflow>
{
    public void Configure(EntityTypeBuilder<Workflow> builder)
    {
        builder.ToTable("Workflows");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasConversion(
                name => name.Value,
                value => WorkflowName.Create(value).Value!)
            .HasMaxLength(WorkflowName.MaxLength)
            .IsRequired();

        builder.Property(x => x.Active)
            .IsRequired();

        builder.Property(x => x.Deleted)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}
