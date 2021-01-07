using System;
using System.ComponentModel.DataAnnotations;

namespace Bumbo.Web.Models.Home
{
    public class SubmitHoursModel
    {
        [Display(Name = "Begintijd")]
        public TimeSpan Start { get; set; }
        
        [Display(Name = "Eindtijd")]
        public TimeSpan End { get; set; }
        
        [Required]
        [Display(Name = "Was u vandaag ziek?")]
        public bool Sick { get; set; } = false;
    }
}