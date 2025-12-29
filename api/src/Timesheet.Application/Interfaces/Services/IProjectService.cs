using Timesheet.Application.DTOs.Project;

namespace Timesheet.Application.Interfaces.Services
{
    /// <summary>
    /// Project Service Interface.
    /// Defines business operations for Project management (Manager role).
    /// </summary>
    public interface IProjectService
    {
        Task<ProjectDto?> GetByIdAsync(int id);
        Task<ProjectDto?> GetByCodeAsync(string code);
        Task<IEnumerable<ProjectDto>> GetAllAsync();
        Task<IEnumerable<ProjectDto>> GetActiveProjectsAsync();
        Task<ProjectDto> CreateAsync(CreateProjectDto dto);
        Task<ProjectDto?> UpdateAsync(int id, UpdateProjectDto dto);
        Task<bool> ActivateAsync(int id);
        Task<bool> DeactivateAsync(int id);
    }
}
