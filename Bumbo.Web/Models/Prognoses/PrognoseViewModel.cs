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
        [Range(0, 999999999999)]
        public int AmountOfCustomers { get; set; }
        [Required]
        [Range(0, 999999999999)]
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

        public int GetEstimatedWorkingHours()
        {
            var freight = this.AmountOfFreight / 100;
            var customers = this.AmountOfCustomers;

            switch (WeatherDescription)
            {
                case "regen":
                    customers = (int)Math.Floor((double)customers * 0.7);
                    break;
                case "zon":
                    customers = (int)Math.Floor((double)customers * 1.2);
                    break;
                case "bewolkt":
                    customers = (int)Math.Floor((double)customers * 0.9);
                    break;
                case "storm":
                    customers = (int)Math.Floor((double)customers * 0.6);
                    break;
                default:
                    break;
            }

            customers /= 50;

            var estimated = customers * freight;

            return estimated < 5 ? 5 : estimated;
        }

        public string GetDayName()
        {
            var culture = new System.Globalization.CultureInfo("nl-NL");

            return culture.TextInfo.ToTitleCase(culture.DateTimeFormat.GetDayName(Date.DayOfWeek)); ;
        }
    }
}
