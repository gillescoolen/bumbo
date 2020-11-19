using System;
using System.Collections.Generic;

namespace Bumbo.Data.Models
{
    public partial class Token
    {
        public int? UserId { get; set; }
        public int TokenId { get; set; }

        public virtual User User { get; set; }
    }
}
