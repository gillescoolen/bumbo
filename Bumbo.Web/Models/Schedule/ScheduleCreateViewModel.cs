using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Bumbo.Data.Models;

namespace Bumbo.Web.Models
{
    public class ScheduleCreateViewModel
    {
        public List<AvailableWorktime> AvailableWorkTimes { get; set; }
        public List<PlannedWorktime> PlannedWorktimes { get; set; }
        public List<Prognoses> Prognoses { get; set; }
        public DateTime MinimumDate { get; set; }
        public DateTime MaximumDate { get; set; }
        public List<string> Errors { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
    }
}
