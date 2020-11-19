using System;
using System.Collections.Generic;

namespace Bumbo.Data.Models
{
    public partial class Branch
    {
        public Branch()
        {
            Norm = new HashSet<Norm>();
            Prognoses = new HashSet<Prognoses>();
            Users = new HashSet<User>();
        }

        public int Id { get; set; }
        public string PostalCode { get; set; }
        public int HouseNumber { get; set; }
        public string HouseNumberLetter { get; set; }
        public string StreetName { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Norm> Norm { get; set; }
        public virtual ICollection<Prognoses> Prognoses { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
