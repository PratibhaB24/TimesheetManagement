using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timesheet.Domain.Common;
using Timesheet.Domain.Enums;

namespace Timesheet.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public ICollection<ProjectAssignment> ProjectAssignments { get; set; } = new List<ProjectAssignment>();
        public ICollection<Timesheet> Timesheets { get; set; } = new List<Timesheet>();
    }
}
