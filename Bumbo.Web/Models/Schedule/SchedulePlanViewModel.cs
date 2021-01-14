using Bumbo.Data;
using Bumbo.Data.Models;
using System;
using System.Collections.Generic;

namespace Bumbo.Web.Models
{
    public class SchedulePlanViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public DateTime Date { get; set; }
        public List<string> Errors { get; set; }
    }
}
