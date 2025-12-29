using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Timesheet.Domain.Entities;

namespace Timesheet.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Entity configuration for TimesheetEntry entity.
    /// 
    /// TimesheetEntry represents a single time entry for a specific date and project.
    /// 
    /// BUSINESS RULES (validated in Application layer):
    /// - Maximum 24 hours per day
    /// - No duplicate entries for same project code and date
    /// 
    /// RELATIONSHIP PATTERN:
    /// Timesheet (1) ----< TimesheetEntry >---- (1) Project
    /// </summary>
    public class TimesheetEntryConfiguration : IEntityTypeConfiguration<TimesheetEntry>
    {
        public void Configure(EntityTypeBuilder<TimesheetEntry> builder)
        {
            builder.ToTable("TimesheetEntries");

            builder.HasKey(te => te.Id);

            builder.Property(te => te.Date)
                .IsRequired();

            builder.Property(te => te.Hours)
                .IsRequired();

            builder.Property(te => te.Description)
                .HasMaxLength(500);

            // RELATIONSHIP: Many TimesheetEntries to One Timesheet
            builder.HasOne(te => te.Timesheet)
                .WithMany(t => t.Entries)
                .HasForeignKey(te => te.TimesheetId)
                .OnDelete(DeleteBehavior.Cascade); // Delete entries when timesheet is deleted

            // RELATIONSHIP: Many TimesheetEntries to One Project
            builder.HasOne(te => te.Project)
                .WithMany(p => p.TimesheetEntries)
                .HasForeignKey(te => te.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // UNIQUE CONSTRAINT: No duplicate entries for same timesheet, project, and date
            // This enforces the business rule at database level
            builder.HasIndex(te => new { te.TimesheetId, te.ProjectId, te.Date })
                .IsUnique();
        }
    }
}
