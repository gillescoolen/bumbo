using System;
using System.Collections.Generic;

namespace Bumbo.Data.Models
{
    public partial class AvailableWorktime
    {
        public int UserId { get; set; }
        public DateTime WorkDate { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan Finish { get; set; }
        public int? SchoolHoursWorked { get; set; }

        public virtual User User { get; set; }
    }
}
