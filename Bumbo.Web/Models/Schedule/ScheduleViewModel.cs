using Bumbo.Data.Models;
using System;
using System.Collections.Generic;

namespace Bumbo.Web.Models
{
    public class ScheduleViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public int UserId { get; set; }
    }
}
