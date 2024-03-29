using Bumbo.Data;
using Bumbo.Data.Models;
using System;
using System.Collections.Generic;

namespace Bumbo.Web.Models
{
    public class ScheduleResponseViewModel
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Title { get; set; }
    }
}
