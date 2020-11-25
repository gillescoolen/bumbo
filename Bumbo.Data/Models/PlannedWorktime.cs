using System;
using System.Collections.Generic;

namespace Bumbo.Data.Models
{
    public partial class PlannedWorktime
    {
        public int UserId { get; set; }
        public DateTime WorkDate { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan Finish { get; set; }
        public string Section { get; set; }
        public virtual User User { get; set; }
    }
}
