using Microsoft.EntityFrameworkCore;
using Timesheet.Application.Interfaces.Repositories;
using Timesheet.Domain.Entities;
using Timesheet.Domain.Enums;
using Timesheet.Infrastructure.Data;

namespace Timesheet.Infrastructure.Repositories
{
    /// <summary>
    /// User Repository Implementation.
    /// Inherits from generic Repository and adds User-specific methods.
    /// </summary>
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(TimesheetDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<IEnumerable<User>> GetActiveEmployeesAsync()
        {
            return await _dbSet
                .Where(u => u.IsActive && u.Role == UserRole.Employee)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetManagersAsync()
        {
            return await _dbSet
                .Where(u => u.Role == UserRole.Manager)
                .ToListAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet
                .AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }
    }
}
