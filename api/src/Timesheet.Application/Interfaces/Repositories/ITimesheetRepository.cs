using Timesheet.Domain.Enums;

namespace Timesheet.Application.Interfaces.Repositories
{
    /// <summary>
    /// Timesheet-specific repository interface.
    /// </summary>
    public interface ITimesheetRepository : IRepository<Domain.Entities.Timesheet>
    {
        Task<Domain.Entities.Timesheet?> GetTimesheetWithEntriesAsync(int timesheetId);
        Task<IEnumerable<Domain.Entities.Timesheet>> GetUserTimesheetsAsync(int userId);
        Task<IEnumerable<Domain.Entities.Timesheet>> GetTimesheetsByStatusAsync(TimesheetStatus status);
        Task<IEnumerable<Domain.Entities.Timesheet>> GetPendingTimesheetsAsync();
    }
}
