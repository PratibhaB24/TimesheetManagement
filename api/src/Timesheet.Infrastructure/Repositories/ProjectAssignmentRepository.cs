using Microsoft.EntityFrameworkCore;
using Timesheet.Application.Interfaces.Repositories;
using Timesheet.Domain.Entities;
using Timesheet.Infrastructure.Data;

namespace Timesheet.Infrastructure.Repositories
{
    /// <summary>
    /// ProjectAssignment Repository Implementation.
    /// </summary>
    public class ProjectAssignmentRepository : Repository<ProjectAssignment>, IProjectAssignmentRepository
    {
        public ProjectAssignmentRepository(TimesheetDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProjectAssignment>> GetUserAssignmentsAsync(int userId)
        {
            return await _dbSet
                .Include(pa => pa.Project)
                .Where(pa => pa.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProjectAssignment>> GetProjectAssignmentsAsync(int projectId)
        {
            return await _dbSet
                .Include(pa => pa.User)
                .Where(pa => pa.ProjectId == projectId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProjectAssignment>> GetActiveAssignmentsForUserAsync(int userId, DateTime date)
        {
            return await _dbSet
                .Include(pa => pa.Project)
                .Where(pa => pa.UserId == userId
                    && pa.StartDate <= date
                    && (pa.EndDate == null || pa.EndDate >= date)
                    && pa.Project!.Status == Domain.Enums.ProjectStatus.Active)
                .ToListAsync();
        }

        public async Task<bool> IsUserAssignedToProjectAsync(int userId, int projectId, DateTime date)
        {
            return await _dbSet
                .AnyAsync(pa => pa.UserId == userId
                    && pa.ProjectId == projectId
                    && pa.StartDate <= date
                    && (pa.EndDate == null || pa.EndDate >= date));
        }
    }
}
