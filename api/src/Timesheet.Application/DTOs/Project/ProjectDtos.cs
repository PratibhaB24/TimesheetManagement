using Timesheet.Domain.Enums;

namespace Timesheet.Application.DTOs.Project
{
    /// <summary>
    /// DTO for returning Project data.
    /// </summary>
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public bool IsBillable { get; set; }
        public ProjectStatus Status { get; set; }
        public string StatusName => Status.ToString();
        public DateTime CreatedOn { get; set; }
    }

    /// <summary>
    /// DTO for creating a new Project.
    /// </summary>
    public class CreateProjectDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public bool IsBillable { get; set; }
    }

    /// <summary>
    /// DTO for updating an existing Project.
    /// </summary>
    public class UpdateProjectDto
    {
        public string Name { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public bool IsBillable { get; set; }
        public ProjectStatus Status { get; set; }
    }
}
