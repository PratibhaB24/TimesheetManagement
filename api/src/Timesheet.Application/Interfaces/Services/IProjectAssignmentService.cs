using Timesheet.Application.DTOs.ProjectAssignment;

namespace Timesheet.Application.Interfaces.Services
{
    /// <summary>
    /// ProjectAssignment Service Interface.
    /// Defines business operations for assigning projects to employees (Manager role).
    /// </summary>
    public interface IProjectAssignmentService
    {
        Task<ProjectAssignmentDto?> GetByIdAsync(int id);
        Task<IEnumerable<ProjectAssignmentDto>> GetAllAsync();
        Task<IEnumerable<ProjectAssignmentDto>> GetUserAssignmentsAsync(int userId);
        Task<IEnumerable<ProjectAssignmentDto>> GetProjectAssignmentsAsync(int projectId);
        Task<ProjectAssignmentDto> CreateAsync(CreateProjectAssignmentDto dto);
        Task<ProjectAssignmentDto?> UpdateAsync(int id, UpdateProjectAssignmentDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> IsUserAssignedToProjectAsync(int userId, int projectId, DateTime date);
    }
}
