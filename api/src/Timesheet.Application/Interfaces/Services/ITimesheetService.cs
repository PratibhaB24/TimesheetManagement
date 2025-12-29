using Timesheet.Application.DTOs.Timesheet;

namespace Timesheet.Application.Interfaces.Services
{
    /// <summary>
    /// Timesheet Service Interface.
    /// Defines business operations for Timesheet management.
    /// </summary>
    public interface ITimesheetService
    {
        Task<TimesheetDto?> GetByIdAsync(int id);
        Task<IEnumerable<TimesheetDto>> GetUserTimesheetsAsync(int userId);
        Task<IEnumerable<TimesheetDto>> GetPendingTimesheetsAsync();
        Task<TimesheetDto> CreateAsync(int userId, CreateTimesheetDto dto);
        Task<TimesheetDto?> AddEntryAsync(int timesheetId, CreateTimesheetEntryDto dto);
        Task<bool> UpdateEntryAsync(int entryId, UpdateTimesheetEntryDto dto);
        Task<bool> DeleteEntryAsync(int entryId);
        Task<bool> SubmitAsync(int timesheetId);
        Task<bool> ApproveAsync(int timesheetId);
        Task<bool> RejectAsync(int timesheetId, string comments);
    }
}
