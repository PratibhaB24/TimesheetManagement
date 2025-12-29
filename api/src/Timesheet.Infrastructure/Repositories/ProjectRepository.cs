using Microsoft.EntityFrameworkCore;
using Timesheet.Application.Interfaces.Repositories;
using Timesheet.Domain.Entities;
using Timesheet.Domain.Enums;
using Timesheet.Infrastructure.Data;

namespace Timesheet.Infrastructure.Repositories
{
    /// <summary>
    /// Project Repository Implementation.
    /// </summary>
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        public ProjectRepository(TimesheetDbContext context) : base(context)
        {
        }

        public async Task<Project?> GetByCodeAsync(string code)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.Code.ToLower() == code.ToLower());
        }

        public async Task<IEnumerable<Project>> GetActiveProjectsAsync()
        {
            return await _dbSet
                .Where(p => p.Status == ProjectStatus.Active)
                .ToListAsync();
        }

        public async Task<bool> CodeExistsAsync(string code)
        {
            return await _dbSet
                .AnyAsync(p => p.Code.ToLower() == code.ToLower());
        }

        public async Task<IEnumerable<Project>> GetBillableProjectsAsync()
        {
            return await _dbSet
                .Where(p => p.IsBillable && p.Status == ProjectStatus.Active)
                .ToListAsync();
        }
    }
}
