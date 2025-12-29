using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timesheet.Domain.Common;
using Timesheet.Domain.Enums;

namespace Timesheet.Domain.Entities
{
    public class Project : BaseEntity
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;        
        public string ClientName { get; set; } = null!;
        public bool IsBillable { get; set; }
        public ProjectStatus Status { get; set; } = ProjectStatus.Active;

        // Navigation Property for EF Core
        public ICollection<ProjectAssignment> Assignments { get; set; } = new List<ProjectAssignment>();
        public ICollection<TimesheetEntry> TimesheetEntries { get; set; } = new List<TimesheetEntry>();
    }
}
