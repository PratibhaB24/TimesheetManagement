using Timesheet.Domain.Entities;
using Timesheet.Domain.Enums;

namespace Timesheet.Application.Factories
{
    /// <summary>
    /// Factory for creating Timesheet and TimesheetEntry objects with validation.
    /// Centralizes creation logic and ensures business rules are applied.
    /// </summary>
    public interface ITimesheetFactory
    {
        Domain.Entities.Timesheet CreateTimesheet(int userId, DateTime submissionDate);
        TimesheetEntry CreateTimesheetEntry(int timesheetId, int projectId, DateTime date, double hours, string description);
    }

    public class TimesheetFactory : ITimesheetFactory
    {
        private const double MAX_HOURS_PER_DAY = 24;
        private const double MIN_HOURS = 0.25; // Minimum 15 minutes

        public Domain.Entities.Timesheet CreateTimesheet(int userId, DateTime submissionDate)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID.", nameof(userId));

            if (submissionDate > DateTime.UtcNow.AddDays(7))
                throw new ArgumentException("Cannot create timesheet for dates too far in the future.", nameof(submissionDate));

            return new Domain.Entities.Timesheet
            {
                UserId = userId,
                SubmissionDate = submissionDate,
                Status = TimesheetStatus.Draft,
                CreatedOn = DateTime.UtcNow
            };
        }

        public TimesheetEntry CreateTimesheetEntry(int timesheetId, int projectId, DateTime date, double hours, string description)
        {
            if (hours < MIN_HOURS)
                throw new ArgumentException($"Hours must be at least {MIN_HOURS} (15 minutes).", nameof(hours));

            if (hours > MAX_HOURS_PER_DAY)
                throw new ArgumentException($"Hours cannot exceed {MAX_HOURS_PER_DAY} per day.", nameof(hours));

            if (date.Date > DateTime.UtcNow.Date)
                throw new ArgumentException("Cannot log hours for future dates.", nameof(date));

            if (timesheetId <= 0)
                throw new ArgumentException("Invalid timesheet ID.", nameof(timesheetId));

            if (projectId <= 0)
                throw new ArgumentException("Invalid project ID.", nameof(projectId));

            return new TimesheetEntry
            {
                TimesheetId = timesheetId,
                ProjectId = projectId,
                Date = date.Date,
                Hours = Math.Round(hours, 2),
                Description = description?.Trim() ?? string.Empty,
                CreatedOn = DateTime.UtcNow
            };
        }
    }
}
