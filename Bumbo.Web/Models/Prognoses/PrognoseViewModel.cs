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
        
    }
}
