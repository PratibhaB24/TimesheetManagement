using Microsoft.EntityFrameworkCore.Storage;
using Timesheet.Application.Interfaces.Repositories;
using Timesheet.Infrastructure.Data;

namespace Timesheet.Infrastructure.Repositories
{
    /// <summary>
    /// Unit of Work Implementation.
    /// 
    /// HOW IT WORKS:
    /// 1. Holds references to all repositories
    /// 2. All repositories share the same DbContext instance
    /// 3. SaveChangesAsync commits all pending changes in one transaction
    /// 4. Supports explicit transactions for complex operations
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TimesheetDbContext _context;
        private IDbContextTransaction? _transaction;

        // Lazy initialization of repositories
        private IUserRepository? _users;
        private IProjectRepository? _projects;
        private IProjectAssignmentRepository? _projectAssignments;
        private ITimesheetRepository? _timesheets;
        private ITimesheetEntryRepository? _timesheetEntries;

        public UnitOfWork(TimesheetDbContext context)
        {
            _context = context;
        }

        // Repository properties with lazy initialization
        // This means repositories are only created when first accessed
        public IUserRepository Users => 
            _users ??= new UserRepository(_context);

        public IProjectRepository Projects => 
            _projects ??= new ProjectRepository(_context);

        public IProjectAssignmentRepository ProjectAssignments => 
            _projectAssignments ??= new ProjectAssignmentRepository(_context);

        public ITimesheetRepository Timesheets => 
            _timesheets ??= new TimesheetRepository(_context);

        public ITimesheetEntryRepository TimesheetEntries => 
            _timesheetEntries ??= new TimesheetEntryRepository(_context);

        /// <summary>
        /// Saves all changes made in this unit of work to the database.
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Begins a new transaction for complex operations.
        /// </summary>
        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        /// <summary>
        /// Rolls back the current transaction.
        /// </summary>
        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        /// <summary>
        /// Disposes the DbContext and transaction.
        /// </summary>
        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
