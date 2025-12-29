using Timesheet.Domain.Entities;

namespace Timesheet.Application.Interfaces.Repositories
{
    /// <summary>
    /// User-specific repository interface.
    /// Extends generic repository with User-specific methods.
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetActiveEmployeesAsync();
        Task<IEnumerable<User>> GetManagersAsync();
        Task<bool> EmailExistsAsync(string email);
    }
}
