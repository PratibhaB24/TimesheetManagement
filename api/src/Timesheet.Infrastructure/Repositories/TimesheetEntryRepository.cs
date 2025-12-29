using Microsoft.EntityFrameworkCore;
using Timesheet.Application.Interfaces.Repositories;
using Timesheet.Domain.Entities;
using Timesheet.Infrastructure.Data;

namespace Timesheet.Infrastructure.Repositories
{
    /// <summary>
    /// TimesheetEntry Repository Implementation.
    /// </summary>
    public class TimesheetEntryRepository : Repository<TimesheetEntry>, ITimesheetEntryRepository
    {
        public TimesheetEntryRepository(TimesheetDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TimesheetEntry>> GetEntriesByTimesheetAsync(int timesheetId)
        {
            return await _dbSet
                .Include(te => te.Project)
                .Where(te => te.TimesheetId == timesheetId)
                .ToListAsync();
        }

        public async Task<double> GetTotalHoursForDateAsync(int timesheetId, DateTime date)
        {
            return await _dbSet
                .Where(te => te.TimesheetId == timesheetId && te.Date.Date == date.Date)
                .SumAsync(te => te.Hours);
        }

        public async Task<bool> EntryExistsAsync(int timesheetId, int projectId, DateTime date)
        {
            return await _dbSet
                .AnyAsync(te => te.TimesheetId == timesheetId 
                    && te.ProjectId == projectId 
                    && te.Date.Date == date.Date);
        }
    }
}
