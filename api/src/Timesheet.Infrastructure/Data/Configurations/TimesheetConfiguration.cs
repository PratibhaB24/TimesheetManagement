using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Timesheet.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Entity configuration for Timesheet entity.
    /// 
    /// Timesheet represents a collection of time entries submitted by an employee.
    /// It has a lifecycle: Draft → Submitted → Approved/Rejected
    /// 
    /// RELATIONSHIP PATTERN:
    /// User (1) ----< Timesheet >---- (*) TimesheetEntry
    /// </summary>
    public class TimesheetConfiguration : IEntityTypeConfiguration<Domain.Entities.Timesheet>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Timesheet> builder)
        {
            builder.ToTable("Timesheets");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.SubmissionDate)
                .IsRequired();

            builder.Property(t => t.Status)
                .IsRequired();

            // Rejection comments are required only when status is Rejected
            // This validation is handled in the Application layer
            builder.Property(t => t.RejectionComments)
                .HasMaxLength(1000);

            // RELATIONSHIP: Many Timesheets to One User
            builder.HasOne(t => t.User)
                .WithMany(u => u.Timesheets)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index on UserId and Status for common queries
            builder.HasIndex(t => new { t.UserId, t.Status });
        }
    }
}
