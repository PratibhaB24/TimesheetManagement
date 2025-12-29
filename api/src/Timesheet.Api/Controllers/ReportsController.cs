using Microsoft.AspNetCore.Mvc;
using Timesheet.Application.Calculations;
using Timesheet.Application.DTOs.Common;
using Timesheet.Application.DTOs.Report;
using Timesheet.Application.Interfaces.Services;

namespace Timesheet.Api.Controllers
{
    /// <summary>
    /// Reports Controller.
    /// Manager role: View various reports with date range filtering.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>
        /// Get employee-wise hours summary.
        /// </summary>
        [HttpGet("employee-hours")]
        public async Task<ActionResult<ApiResponse<IEnumerable<EmployeeHoursSummaryDto>>>> GetEmployeeHoursSummary(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] int? userId = null)
        {
            var filter = new ReportFilterDto
            {
                StartDate = startDate,
                EndDate = endDate,
                UserId = userId
            };

            var report = await _reportService.GetEmployeeHoursSummaryAsync(filter);
            return Ok(ApiResponse<IEnumerable<EmployeeHoursSummaryDto>>.SuccessResponse(report));
        }

        /// <summary>
        /// Get project-wise hours summary.
        /// </summary>
        [HttpGet("project-hours")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectHoursSummaryDto>>>> GetProjectHoursSummary(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] int? projectId = null)
        {
            var filter = new ReportFilterDto
            {
                StartDate = startDate,
                EndDate = endDate,
                ProjectId = projectId
            };

            var report = await _reportService.GetProjectHoursSummaryAsync(filter);
            return Ok(ApiResponse<IEnumerable<ProjectHoursSummaryDto>>.SuccessResponse(report));
        }

        /// <summary>
        /// Get billable vs non-billable hours report.
        /// </summary>
        [HttpGet("billable")]
        public async Task<ActionResult<ApiResponse<BillableReportDto>>> GetBillableReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var filter = new ReportFilterDto
            {
                StartDate = startDate,
                EndDate = endDate
            };

            var report = await _reportService.GetBillableReportAsync(filter);
            return Ok(ApiResponse<BillableReportDto>.SuccessResponse(report));
        }

        /// <summary>
        /// Calculate hours using different calculation methods.
        /// Types: Standard (0), Overtime (1), BillableOnly (2), WeeklyCapped (3)
        /// </summary>
        [HttpGet("calculate-hours")]
        public async Task<ActionResult<ApiResponse<HoursCalculationResultDto>>> CalculateHours(
            [FromQuery] int userId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] CalculationType calculationType = CalculationType.Standard)
        {
            try
            {
                var result = await _reportService.CalculateHoursAsync(userId, startDate, endDate, calculationType);
                return Ok(ApiResponse<HoursCalculationResultDto>.SuccessResponse(result));
            }
            catch (ArgumentException ex)
            {
                return NotFound(ApiResponse<HoursCalculationResultDto>.ErrorResponse(ex.Message));
            }
        }
    }
}
