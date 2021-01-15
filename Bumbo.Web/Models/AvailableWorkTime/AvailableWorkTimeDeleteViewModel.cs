using System;
using System.Collections.Generic;
using Bumbo.Data.Models;

namespace Bumbo.Web.Models
{
    public class AvailableWorkTimeDeleteViewModel
    {
        public int UserId { get; set; }
        public AvailableWorktime AvailableWorkTime { get; set; }
    }
}
