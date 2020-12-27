using System;
using System.Collections.Generic;

namespace Bumbo.Data.Models
{
    public partial class BranchViewModel
    {
        public int Id { get; set; }
        public string PostalCode { get; set; }
        public int HouseNumber { get; set; }
        public string HouseNumberLetter { get; set; }
        public string StreetName { get; set; }
        public string Name { get; set; }
    }
}
