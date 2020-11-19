using System;
using System.Collections.Generic;

namespace Bumbo.Data.Models
{
    public partial class FurloughRequest
    {
        public int UserId { get; set; }
        public DateTime WorkDate { get; set; }
        public byte IsApproved { get; set; }

        public virtual User User { get; set; }
    }
}
