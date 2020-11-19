using System;
using System.Collections.Generic;

namespace Bumbo.Data.Models
{
    public partial class ActualTimeWorked
    {
        public int UserId { get; set; }
        public DateTime WorkDate { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan Finish { get; set; }
        public byte? Sickness { get; set; }

        public virtual User User { get; set; }
    }
}
