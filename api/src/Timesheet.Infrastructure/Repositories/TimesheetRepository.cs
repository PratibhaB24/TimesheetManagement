using Microsoft.EntityFrameworkCore;
using Timesheet.Application.Interfaces.Repositories;
using Timesheet.Domain.Enums;
using Timesheet.Infrastructure.Data;

namespace Timesheet.Infrastructure.Repositories
{
    /// <summary>
    /// Timesheet Repository Implementation.
    /// </summary>
    public class TimesheetRepository : Repository<Domain.Entities.Timesheet>, ITimesheetRepository
    {
        public TimesheetRepository(TimesheetDbContext context) : base(context)
        {
        }

        public async Task<Domain.Entities.Timesheet?> GetTimesheetWithEntriesAsync(int timesheetId)
        {
            return await _dbSet
                .Include(t => t.Entries)
                    .ThenInclude(e => e.Project)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == timesheetId);
        }

        public async Task<IEnumerable<Domain.Entities.Timesheet>> GetUserTimesheetsAsync(int userId)
        {
            return await _dbSet
                .Include(t => t.Entries)
                    .ThenInclude(e => e.Project)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.SubmissionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Timesheet>> GetTimesheetsByStatusAsync(TimesheetStatus status)
        {
            return await _dbSet
                .Include(t => t.User)
                .Include(t => t.Entries)
                    .ThenInclude(e => e.Project)
                .Where(t => t.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Timesheet>> GetPendingTimesheetsAsync()
        {
            return await GetTimesheetsByStatusAsync(TimesheetStatus.Submitted);
        }
    }
}
