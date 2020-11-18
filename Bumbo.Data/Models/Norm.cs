using System;
using System.Collections.Generic;

namespace Bumbo.Data.Models
{
    public partial class Norm
    {
        public string Activity { get; set; }
        public int Norm1 { get; set; }
        public string NormDescription { get; set; }
        public int BranchId { get; set; }

        public virtual Branch Branch { get; set; }
    }
}
