using System;
using System.Collections.Generic;

namespace Bumbo.Data.Models
{
    public partial class PrognoseViewModel
    {
        public DateTime Date { get; set; }
        public int AmountOfCustomers { get; set; }
        public int AmountOfFreight { get; set; }
        public int BranchId { get; set; }
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

    }
}
