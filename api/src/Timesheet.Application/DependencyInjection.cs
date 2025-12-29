using Microsoft.Extensions.DependencyInjection;
using Timesheet.Application.Calculations;
using Timesheet.Application.Factories;
using Timesheet.Application.Interfaces.Services;
using Timesheet.Application.Mappings;
using Timesheet.Application.Services;

namespace Timesheet.Application
{
    /// <summary>
    /// Extension methods for registering Application services.
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register AutoMapper with the mapping profile
            services.AddAutoMapper(typeof(MappingProfile));

            // Factory for creating timesheet objects with validation
            services.AddScoped<ITimesheetFactory, TimesheetFactory>();

            // Hours calculation strategies
            services.AddSingleton<IHoursCalculationSelector, HoursCalculationSelector>();

            // Register the base timesheet service
            services.AddScoped<TimesheetService>();
            
            // Wrap with logging and validation
            services.AddScoped<ITimesheetService>(provider =>
            {
                var baseService = provider.GetRequiredService<TimesheetService>();
                var logger = provider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<LoggingTimesheetService>>();
                
                // Chain: Client → Validation → Logging → BaseService
                var loggingService = new LoggingTimesheetService(baseService, logger);
                var validatingService = new ValidatingTimesheetService(loggingService);
                
                return validatingService;
            });

            // Register other services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IProjectAssignmentService, ProjectAssignmentService>();
            services.AddScoped<IReportService, ReportService>();

            return services;
        }
    }
}
