using Microsoft.EntityFrameworkCore;
using Timesheet.Application.Calculations;
using Timesheet.Application.DTOs.Report;
using Timesheet.Application.Interfaces.Repositories;
using Timesheet.Application.Interfaces.Services;
using Timesheet.Domain.Enums;

namespace Timesheet.Application.Services
{
    /// <summary>
    /// Report Service Implementation.
    /// All reports use EF Core and LINQ queries for aggregations.
    /// </summary>
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHoursCalculationSelector _calculationSelector;

        public ReportService(
            IUnitOfWork unitOfWork,
            IHoursCalculationSelector calculationSelector)
        {
            _unitOfWork = unitOfWork;
            _calculationSelector = calculationSelector;
        }

        /// <summary>
        /// Employee-wise hours summary report.
        /// Shows total hours, billable/non-billable breakdown per employee.
        /// </summary>
        public async Task<IEnumerable<EmployeeHoursSummaryDto>> GetEmployeeHoursSummaryAsync(ReportFilterDto filter)
        {
            // Using LINQ query with EF Core
            var query = _unitOfWork.TimesheetEntries.Query()
                .Include(te => te.Timesheet)
                    .ThenInclude(t => t!.User)
                .Include(te => te.Project)
                .Where(te => te.Timesheet!.Status == TimesheetStatus.Approved)
                .Where(te => te.Date >= filter.StartDate && te.Date <= filter.EndDate);

            // Optional user filter
            if (filter.UserId.HasValue)
            {
                query = query.Where(te => te.Timesheet!.UserId == filter.UserId.Value);
            }

            var result = await query
                .GroupBy(te => new { te.Timesheet!.UserId, te.Timesheet.User!.FullName })
                .Select(g => new EmployeeHoursSummaryDto
                {
                    UserId = g.Key.UserId,
                    EmployeeName = g.Key.FullName,
                    TotalHours = g.Sum(te => te.Hours),
                    BillableHours = g.Where(te => te.Project!.IsBillable).Sum(te => te.Hours),
                    NonBillableHours = g.Where(te => !te.Project!.IsBillable).Sum(te => te.Hours),
                    ApprovedTimesheets = g.Select(te => te.TimesheetId).Distinct().Count()
                })
                .OrderBy(e => e.EmployeeName)
                .ToListAsync();

            return result;
        }

        /// <summary>
        /// Project-wise hours summary report.
        /// Shows total hours and employee count per project.
        /// </summary>
        public async Task<IEnumerable<ProjectHoursSummaryDto>> GetProjectHoursSummaryAsync(ReportFilterDto filter)
        {
            var query = _unitOfWork.TimesheetEntries.Query()
                .Include(te => te.Timesheet)
                .Include(te => te.Project)
                .Where(te => te.Timesheet!.Status == TimesheetStatus.Approved)
                .Where(te => te.Date >= filter.StartDate && te.Date <= filter.EndDate);

            // Optional project filter
            if (filter.ProjectId.HasValue)
            {
                query = query.Where(te => te.ProjectId == filter.ProjectId.Value);
            }

            var result = await query
                .GroupBy(te => new 
                { 
                    te.ProjectId, 
                    te.Project!.Code, 
                    te.Project.Name, 
                    te.Project.ClientName,
                    te.Project.IsBillable 
                })
                .Select(g => new ProjectHoursSummaryDto
                {
                    ProjectId = g.Key.ProjectId,
                    ProjectCode = g.Key.Code,
                    ProjectName = g.Key.Name,
                    ClientName = g.Key.ClientName,
                    IsBillable = g.Key.IsBillable,
                    TotalHours = g.Sum(te => te.Hours),
                    EmployeeCount = g.Select(te => te.Timesheet!.UserId).Distinct().Count()
                })
                .OrderBy(p => p.ProjectCode)
                .ToListAsync();

            return result;
        }

        /// <summary>
        /// Billable vs Non-billable hours report.
        /// Provides breakdown of billable and non-billable hours.
        /// </summary>
        public async Task<BillableReportDto> GetBillableReportAsync(ReportFilterDto filter)
        {
            var query = _unitOfWork.TimesheetEntries.Query()
                .Include(te => te.Timesheet)
                .Include(te => te.Project)
                .Where(te => te.Timesheet!.Status == TimesheetStatus.Approved)
                .Where(te => te.Date >= filter.StartDate && te.Date <= filter.EndDate);

            // Get details grouped by project
            var details = await query
                .GroupBy(te => new { te.Project!.Code, te.Project.Name, te.Project.IsBillable })
                .Select(g => new BillableDetailDto
                {
                    ProjectCode = g.Key.Code,
                    ProjectName = g.Key.Name,
                    IsBillable = g.Key.IsBillable,
                    Hours = g.Sum(te => te.Hours)
                })
                .OrderByDescending(d => d.Hours)
                .ToListAsync();

            var totalBillable = details.Where(d => d.IsBillable).Sum(d => d.Hours);
            var totalNonBillable = details.Where(d => !d.IsBillable).Sum(d => d.Hours);
            var totalHours = totalBillable + totalNonBillable;

            return new BillableReportDto
            {
                TotalBillableHours = totalBillable,
                TotalNonBillableHours = totalNonBillable,
                TotalHours = totalHours,
                BillablePercentage = totalHours > 0 ? Math.Round((totalBillable / totalHours) * 100, 2) : 0,
                Details = details
            };
        }

        /// <summary>
        /// Calculate hours using different calculation methods.
        /// </summary>
        public async Task<HoursCalculationResultDto> CalculateHoursAsync(
            int userId, 
            DateTime startDate, 
            DateTime endDate, 
            CalculationType calculationType)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException($"User with ID {userId} not found.");

            var entries = await _unitOfWork.TimesheetEntries.Query()
                .Include(te => te.Timesheet)
                .Include(te => te.Project)
                .Where(te => te.Timesheet!.UserId == userId)
                .Where(te => te.Timesheet!.Status == TimesheetStatus.Approved)
                .Where(te => te.Date >= startDate && te.Date <= endDate)
                .ToListAsync();

            var calculation = _calculationSelector.GetCalculation(calculationType);

            var rawHours = entries.Sum(e => e.Hours);
            var calculatedHours = calculation.CalculateHours(entries);
            
            const double hourlyRate = 50.0;
            var billableAmount = calculation.CalculateBillableAmount(entries, hourlyRate);

            var description = calculationType switch
            {
                CalculationType.Standard => "Simple sum of all hours worked.",
                CalculationType.Overtime => "Hours over 8/day are counted at 1.5x rate.",
                CalculationType.BillableOnly => "Only hours from billable projects are counted.",
                CalculationType.WeeklyCapped => "Maximum 40 hours per week.",
                _ => "Unknown calculation type"
            };

            return new HoursCalculationResultDto
            {
                UserId = userId,
                EmployeeName = user.FullName,
                StartDate = startDate,
                EndDate = endDate,
                StrategyUsed = calculation.StrategyName,
                RawHours = rawHours,
                CalculatedHours = calculatedHours,
                HourlyRate = hourlyRate,
                BillableAmount = billableAmount,
                Description = description
            };
        }
    }
}
