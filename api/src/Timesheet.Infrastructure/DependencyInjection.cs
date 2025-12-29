using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Timesheet.Application.Interfaces.Repositories;
using Timesheet.Infrastructure.Data;
using Timesheet.Infrastructure.Repositories;

namespace Timesheet.Infrastructure
{
    /// <summary>
    /// Extension methods for registering Infrastructure services.
    /// 
    /// WHY EXTENSION METHOD?
    /// - Keeps Program.cs clean
    /// - Groups related registrations together
    /// - Easy to reuse across projects
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            // Register DbContext with SQL Server
            services.AddDbContext<TimesheetDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(TimesheetDbContext).Assembly.FullName)));

            // Register Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register Repositories (if needed individually)
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IProjectAssignmentRepository, ProjectAssignmentRepository>();
            services.AddScoped<ITimesheetRepository, TimesheetRepository>();
            services.AddScoped<ITimesheetEntryRepository, TimesheetEntryRepository>();

            return services;
        }
    }
}
