using Bumbo.Data;
using Bumbo.Data.Models;
using System;
using System.Collections.Generic;

namespace Bumbo.Web.Models
{
    public class PlanViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public DateTime Date { get; set; }
    }
}
