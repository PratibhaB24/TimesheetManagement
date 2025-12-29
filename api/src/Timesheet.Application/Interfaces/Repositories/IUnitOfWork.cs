namespace Timesheet.Application.Interfaces.Repositories
{
    /// <summary>
    /// Unit of Work Pattern Interface.
    /// 
    /// WHY UNIT OF WORK?
    /// - Coordinates the work of multiple repositories
    /// - Ensures all changes are saved in a single transaction
    /// - Provides a single point to commit changes
    /// 
    /// EXAMPLE USAGE:
    /// await _unitOfWork.Users.AddAsync(newUser);
    /// await _unitOfWork.Projects.AddAsync(newProject);
    /// await _unitOfWork.SaveChangesAsync(); // Both saved in one transaction
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IProjectRepository Projects { get; }
        IProjectAssignmentRepository ProjectAssignments { get; }
        ITimesheetRepository Timesheets { get; }
        ITimesheetEntryRepository TimesheetEntries { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
