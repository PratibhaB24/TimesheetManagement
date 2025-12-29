namespace Timesheet.Application.DTOs.Report
{
    /// <summary>
    /// DTO for Employee-wise hours summary report.
    /// </summary>
    public class EmployeeHoursSummaryDto
    {
        public int UserId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public double TotalHours { get; set; }
        public double BillableHours { get; set; }
        public double NonBillableHours { get; set; }
        public int ApprovedTimesheets { get; set; }
    }

    /// <summary>
    /// DTO for Project-wise hours summary report.
    /// </summary>
    public class ProjectHoursSummaryDto
    {
        public int ProjectId { get; set; }
        public string ProjectCode { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public bool IsBillable { get; set; }
        public double TotalHours { get; set; }
        public int EmployeeCount { get; set; }
    }

    /// <summary>
    /// DTO for Billable vs Non-billable hours report.
    /// </summary>
    public class BillableReportDto
    {
        public double TotalBillableHours { get; set; }
        public double TotalNonBillableHours { get; set; }
        public double TotalHours { get; set; }
        public double BillablePercentage { get; set; }
        public List<BillableDetailDto> Details { get; set; } = new();
    }

    /// <summary>
    /// Detail item for billable report.
    /// </summary>
    public class BillableDetailDto
    {
        public string ProjectCode { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public bool IsBillable { get; set; }
        public double Hours { get; set; }
    }

    /// <summary>
    /// DTO for date range filter used in reports.
    /// </summary>
    public class ReportFilterDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? UserId { get; set; }
        public int? ProjectId { get; set; }
    }

    /// <summary>
    /// DTO for hours calculation result using Strategy Pattern.
    /// Shows how different calculation strategies affect the result.
    /// </summary>
    public class HoursCalculationResultDto
    {
        public int UserId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string StrategyUsed { get; set; } = string.Empty;
        public double RawHours { get; set; }
        public double CalculatedHours { get; set; }
        public double BillableAmount { get; set; }
        public double HourlyRate { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
