using Timesheet.Domain.Enums;

namespace Timesheet.Application.DTOs.Timesheet
{
    /// <summary>
    /// DTO for returning Timesheet data.
    /// </summary>
    public class TimesheetDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime SubmissionDate { get; set; }
        public TimesheetStatus Status { get; set; }
        public string StatusName => Status.ToString();
        public string? RejectionComments { get; set; }
        public double TotalHours { get; set; }
        public List<TimesheetEntryDto> Entries { get; set; } = new();
    }

    /// <summary>
    /// DTO for creating a new Timesheet.
    /// </summary>
    public class CreateTimesheetDto
    {
        public DateTime SubmissionDate { get; set; }
        public List<CreateTimesheetEntryDto> Entries { get; set; } = new();
    }

    /// <summary>
    /// DTO for submitting a timesheet (changing status to Submitted).
    /// </summary>
    public class SubmitTimesheetDto
    {
        public int TimesheetId { get; set; }
    }

    /// <summary>
    /// DTO for approving/rejecting a timesheet (Manager action).
    /// </summary>
    public class ApproveRejectTimesheetDto
    {
        public int TimesheetId { get; set; }
        public bool IsApproved { get; set; }
        public string? RejectionComments { get; set; } // Required if IsApproved = false
    }

    /// <summary>
    /// DTO for returning TimesheetEntry data.
    /// </summary>
    public class TimesheetEntryDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectCode { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public double Hours { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for creating a new TimesheetEntry.
    /// </summary>
    public class CreateTimesheetEntryDto
    {
        public int ProjectId { get; set; }
        public DateTime Date { get; set; }
        public double Hours { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for updating an existing TimesheetEntry.
    /// </summary>
    public class UpdateTimesheetEntryDto
    {
        public double Hours { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
