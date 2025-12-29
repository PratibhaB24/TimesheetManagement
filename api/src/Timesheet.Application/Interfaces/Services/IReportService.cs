using Timesheet.Application.Calculations;
using Timesheet.Application.DTOs.Report;

namespace Timesheet.Application.Interfaces.Services
{
    /// <summary>
    /// Report Service Interface.
    /// Defines business operations for generating reports (Manager role).
    /// All aggregations use EF Core and LINQ queries as per requirements.
    /// </summary>
    public interface IReportService
    {
        Task<IEnumerable<EmployeeHoursSummaryDto>> GetEmployeeHoursSummaryAsync(ReportFilterDto filter);
        Task<IEnumerable<ProjectHoursSummaryDto>> GetProjectHoursSummaryAsync(ReportFilterDto filter);
        Task<BillableReportDto> GetBillableReportAsync(ReportFilterDto filter);
        
        /// <summary>
        /// Calculate hours for a user using different calculation methods.
        /// </summary>
        Task<HoursCalculationResultDto> CalculateHoursAsync(
            int userId, 
            DateTime startDate, 
            DateTime endDate, 
            CalculationType calculationType);
    }
}
