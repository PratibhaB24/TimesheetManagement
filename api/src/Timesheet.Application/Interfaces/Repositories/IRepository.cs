using System.Linq.Expressions;

namespace Timesheet.Application.Interfaces.Repositories
{
    /// <summary>
    /// Generic Repository Interface following Repository Pattern.
    /// 
    /// WHY REPOSITORY PATTERN?
    /// - Abstracts data access logic from business logic
    /// - Makes code testable (can mock repositories)
    /// - Single place for data access methods
    /// - Application layer doesn't depend on EF Core directly
    /// 
    /// GENERIC TYPE T:
    /// - T is constrained to classes only
    /// - This interface works with any entity type
    /// </summary>
    public interface IRepository<T> where T : class
    {
        // READ Operations
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        // CREATE Operations
        Task<T> AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        // UPDATE Operations
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);

        // DELETE Operations
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

        // QUERYABLE (for complex queries)
        IQueryable<T> Query();
    }
}
