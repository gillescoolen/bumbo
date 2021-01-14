using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bumbo.Data.Models
{
    public partial class PrognoseViewModel
    {
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public int AmountOfCustomers { get; set; }
        [Required]
        public int AmountOfFreight { get; set; }
        [Required]
        public int BranchId { get; set; }
        [Required]
        public string WeatherDescription { get; set; }
        public virtual Branch Branch { get; set; }

        public string Holiday { get; set; }
        public int LastYearVisitors { get; set; }
        public int LastWeekVisitors { get; set; }

        public string GetFormattedDate()
        {
            string day = (Date.Day < 10) ? "0" + Date.Day.ToString() : Date.Day.ToString();
            string month = (Date.Month < 10) ? "0" + Date.Month.ToString() : Date.Month.ToString();

            return day + "-" + month + "-" + Date.Year;
        }

        public string GetDayName()
        {
            var culture = new System.Globalization.CultureInfo("nl-NL");

            return culture.TextInfo.ToTitleCase(culture.DateTimeFormat.GetDayName(Date.DayOfWeek)); ;
        }
    }
}
