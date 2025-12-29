namespace Timesheet.Application.DTOs.ProjectAssignment
{
    /// <summary>
    /// DTO for returning ProjectAssignment data.
    /// </summary>
    public class ProjectAssignmentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public string ProjectCode { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    /// <summary>
    /// DTO for creating a new ProjectAssignment.
    /// </summary>
    public class CreateProjectAssignmentDto
    {
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    /// <summary>
    /// DTO for updating an existing ProjectAssignment.
    /// </summary>
    public class UpdateProjectAssignmentDto
    {
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
