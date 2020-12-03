using System;
using System.Collections.Generic;
using Bumbo.Data;

namespace Bumbo.Web.Models
{
    public class AvailableWorkTimeViewModel
    {
        public int UserId { get; set; }
        public DateTime WorkDate { get; set; }
        public int? SchoolHoursWorked { get; set; }
        public List<TimeSpan> Start { get; set; }
        public List<TimeSpan> Finish { get; set; }
        public List<DateTime> Dates { get; set; }
    }
}
