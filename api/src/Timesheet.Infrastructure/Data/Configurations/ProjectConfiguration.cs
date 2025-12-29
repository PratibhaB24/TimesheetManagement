using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Timesheet.Domain.Entities;

namespace Timesheet.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Entity configuration for Project entity.
    /// 
    /// Projects are managed by Managers and can be:
    /// - Active or Inactive
    /// - Billable or Non-billable
    /// </summary>
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("Projects");

            builder.HasKey(p => p.Id);

            // Project Code must be unique (like "PRJ001", "DEV2024")
            builder.Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.ClientName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.IsBillable)
                .HasDefaultValue(false);

            builder.Property(p => p.Status)
                .IsRequired();

            // Unique index on Code - no duplicate project codes allowed
            builder.HasIndex(p => p.Code)
                .IsUnique();
        }
    }
}
