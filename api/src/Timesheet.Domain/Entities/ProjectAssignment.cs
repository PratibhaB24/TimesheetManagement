using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timesheet.Domain.Common;
using Timesheet.Domain.Enums;

namespace Timesheet.Domain.Entities
{
    public class ProjectAssignment : BaseEntity
    {
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public User? User { get; set; }
        public Project? Project { get; set; }
    }
}
