using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Bumbo.Data.Models;

namespace Bumbo.Web.Models
{
    public class ScheduleCreateViewModel
    {
        [Required]
        public User User { get; set; }
        [Required]
        public List<AvailableWorktime> AvailableWorkTimes { get; set; }
        [Required]
        public List<PlannedWorktime> PlannedWorktimes { get; set; }
        [Required]
        public List<Prognoses> Prognoses { get; set; }
        [Required]
        public DateTime MinimumDate { get; set; }
        [Required]
        public DateTime MaximumDate { get; set; }
    }
}
