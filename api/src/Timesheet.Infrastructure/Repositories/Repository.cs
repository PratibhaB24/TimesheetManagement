using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Timesheet.Application.Interfaces.Repositories;
using Timesheet.Infrastructure.Data;

namespace Timesheet.Infrastructure.Repositories
{
    /// <summary>
    /// Generic Repository Implementation using Entity Framework Core.
    /// 
    /// HOW IT WORKS:
    /// - Uses DbContext to access the database
    /// - DbSet<T> provides access to entities of type T
    /// - All database operations go through EF Core
    /// </summary>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly TimesheetDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(TimesheetDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        // READ: Get entity by primary key
        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        // READ: Get all entities
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        // READ: Find entities matching a condition
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        // READ: Get first entity matching condition or null
        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        // READ: Check if any entity matches condition
        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        // CREATE: Add a new entity
        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        // CREATE: Add multiple entities
        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        // UPDATE: Mark entity as modified
        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        // UPDATE: Mark multiple entities as modified
        public virtual void UpdateRange(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        // DELETE: Remove an entity
        public virtual void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        // DELETE: Remove multiple entities
        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        // QUERY: Get IQueryable for complex queries
        public virtual IQueryable<T> Query()
        {
            return _dbSet.AsQueryable();
        }
    }
}
