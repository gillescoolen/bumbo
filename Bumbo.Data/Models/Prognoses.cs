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
        
    }

}
