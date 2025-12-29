using Timesheet.Domain.Entities;

namespace Timesheet.Application.Interfaces.Repositories
{
    /// <summary>
    /// Project-specific repository interface.
    /// </summary>
    public interface IProjectRepository : IRepository<Project>
    {
        Task<Project?> GetByCodeAsync(string code);
        Task<IEnumerable<Project>> GetActiveProjectsAsync();
        Task<bool> CodeExistsAsync(string code);
        Task<IEnumerable<Project>> GetBillableProjectsAsync();
    }
}
