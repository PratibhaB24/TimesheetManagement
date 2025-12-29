using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timesheet.Domain.Common;
using Timesheet.Domain.Enums;

namespace Timesheet.Domain.Entities
{
    public class Timesheet : BaseEntity
    {
        public int UserId { get; set; }
        public User? User { get; set; }

        public DateTime SubmissionDate { get; set; }
        public TimesheetStatus Status { get; set; }

        // Requirement: Rejection requires mandatory comments
        public string? RejectionComments { get; set; }

        // Child Collection
        public ICollection<TimesheetEntry> Entries { get; set; } = new List<TimesheetEntry>();
    }
}
