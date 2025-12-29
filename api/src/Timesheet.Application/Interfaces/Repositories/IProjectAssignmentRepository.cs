using Timesheet.Domain.Entities;

namespace Timesheet.Application.Interfaces.Repositories
{
    /// <summary>
    /// ProjectAssignment-specific repository interface.
    /// </summary>
    public interface IProjectAssignmentRepository : IRepository<ProjectAssignment>
    {
        Task<IEnumerable<ProjectAssignment>> GetAllWithDetailsAsync();
        Task<ProjectAssignment?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<ProjectAssignment>> GetUserAssignmentsAsync(int userId);
        Task<IEnumerable<ProjectAssignment>> GetProjectAssignmentsAsync(int projectId);
        Task<IEnumerable<ProjectAssignment>> GetActiveAssignmentsForUserAsync(int userId, DateTime date);
        Task<bool> IsUserAssignedToProjectAsync(int userId, int projectId, DateTime date);
    }
}
