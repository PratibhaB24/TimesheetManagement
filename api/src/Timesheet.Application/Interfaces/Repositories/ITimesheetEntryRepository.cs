using Timesheet.Domain.Entities;

namespace Timesheet.Application.Interfaces.Repositories
{
    /// <summary>
    /// TimesheetEntry-specific repository interface.
    /// </summary>
    public interface ITimesheetEntryRepository : IRepository<TimesheetEntry>
    {
        Task<IEnumerable<TimesheetEntry>> GetEntriesByTimesheetAsync(int timesheetId);
        Task<double> GetTotalHoursForDateAsync(int timesheetId, DateTime date);
        Task<bool> EntryExistsAsync(int timesheetId, int projectId, DateTime date);
    }
}
