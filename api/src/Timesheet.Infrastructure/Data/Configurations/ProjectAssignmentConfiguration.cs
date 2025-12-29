using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Timesheet.Domain.Entities;

namespace Timesheet.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Entity configuration for ProjectAssignment entity.
    /// 
    /// ProjectAssignment is a MANY-TO-MANY relationship between User and Project
    /// with additional properties (StartDate, EndDate), so we model it as an entity.
    /// 
    /// RELATIONSHIP PATTERN:
    /// User (1) ----< ProjectAssignment >---- (1) Project
    /// - One User can be assigned to many Projects
    /// - One Project can have many Users assigned
    /// </summary>
    public class ProjectAssignmentConfiguration : IEntityTypeConfiguration<ProjectAssignment>
    {
        public void Configure(EntityTypeBuilder<ProjectAssignment> builder)
        {
            builder.ToTable("ProjectAssignments");

            builder.HasKey(pa => pa.Id);

            builder.Property(pa => pa.StartDate)
                .IsRequired();

            builder.Property(pa => pa.EndDate)
                .IsRequired(false); // EndDate is optional (assignment can be ongoing)

            // RELATIONSHIP: Many ProjectAssignments to One User
            builder.HasOne(pa => pa.User)
                .WithMany(u => u.ProjectAssignments)
                .HasForeignKey(pa => pa.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Don't cascade delete

            // RELATIONSHIP: Many ProjectAssignments to One Project
            builder.HasOne(pa => pa.Project)
                .WithMany(p => p.Assignments)
                .HasForeignKey(pa => pa.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // Composite unique index to prevent duplicate assignments
            // (same user cannot be assigned to same project twice with same start date)
            builder.HasIndex(pa => new { pa.UserId, pa.ProjectId, pa.StartDate })
                .IsUnique();
        }
    }
}
