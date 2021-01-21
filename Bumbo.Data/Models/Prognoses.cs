using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bumbo.Data.Models
{
    public partial class Prognoses
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

        public Prognoses()
        {
            WeatherDescription = "Default";
        }


        public string GetDayName()
        {
            var culture = new System.Globalization.CultureInfo("nl-NL");

            return culture.TextInfo.ToTitleCase(culture.DateTimeFormat.GetDayName(Date.DayOfWeek)); ;
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
    }

}
