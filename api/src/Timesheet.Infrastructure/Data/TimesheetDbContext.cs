using Microsoft.EntityFrameworkCore;
using Timesheet.Domain.Entities;

namespace Timesheet.Infrastructure.Data
{
    /// <summary>
    /// Entity Framework DbContext for Timesheet Management System.
    /// This is the main class that coordinates Entity Framework functionality for your data model.
    /// 
    /// CODE FIRST APPROACH:
    /// - We define our domain entities as classes (already done in Domain layer)
    /// - EF Core will generate database tables based on these entities
    /// - We use migrations to evolve the database schema over time
    /// </summary>
    public class TimesheetDbContext : DbContext
    {
        public TimesheetDbContext(DbContextOptions<TimesheetDbContext> options) : base(options)
        {
        }

        // DbSet properties represent tables in the database
        // Each DbSet<T> maps to a table where T is the entity type
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectAssignment> ProjectAssignments { get; set; }
        public DbSet<Domain.Entities.Timesheet> Timesheets { get; set; }
        public DbSet<TimesheetEntry> TimesheetEntries { get; set; }

        /// <summary>
        /// OnModelCreating is where we configure the entity mappings using Fluent API.
        /// This method is called when the model for a derived context has been initialized.
        /// 
        /// WHY FLUENT API?
        /// - More powerful than Data Annotations
        /// - Keeps domain entities clean (no EF-specific attributes)
        /// - All configuration in one place
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all entity configurations from the assembly
            // This looks for all classes implementing IEntityTypeConfiguration<T>
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TimesheetDbContext).Assembly);
        }

        /// <summary>
        /// Override SaveChangesAsync to automatically set audit fields (CreatedOn, UpdatedOn)
        /// </summary>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<Domain.Common.BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedOn = DateTime.UtcNow;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
