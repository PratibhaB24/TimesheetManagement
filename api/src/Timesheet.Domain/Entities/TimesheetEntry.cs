using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timesheet.Domain.Common;
using Timesheet.Domain.Enums;

namespace Timesheet.Domain.Entities
{
    public class TimesheetEntry : BaseEntity
    {
        public int TimesheetId { get; set; }
        public Timesheet? Timesheet { get; set; }

        public int ProjectId { get; set; }
        public Project? Project { get; set; }

        public DateTime Date { get; set; }

        // Requirement: Max 24 hours per day (Validated in Factory/Service)
        public double Hours { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}
