using Bumbo.Data;
using Bumbo.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Threading.Tasks;

namespace Bumbo.Data
{
    public class User : IdentityUser<int>
    {
        public User()
        {
            ActualTimeWorked = new HashSet<ActualTimeWorked>();
            AvailableWorktime = new HashSet<AvailableWorktime>();
            FurloughRequest = new HashSet<FurloughRequest>();
            PlannedWorktime = new HashSet<PlannedWorktime>();
            Token = new HashSet<Token>();
        }

        /// <summary>
        /// Gets or sets the first name for this user.
        /// </summary>
        [PersonalData]
        [Display(Name = "Telefoonnummer")]
        public int? PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the first name for this user.
        /// </summary>
        [PersonalData]
        [Required]
        [Display(Name = "Voornaam")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name for this user.
        /// </summary>
        [PersonalData]
        [Required]
        [Display(Name = "Achternaam")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the date of birth for this user.
        /// </summary>
        [PersonalData]
        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        [Display(Name = "Geboortedatum")]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the postal code for this user.
        /// </summary>
        [PersonalData]
        [Required]
        [DataType(DataType.PostalCode)]
        [Display(Name = "Postcode")]
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the house number for this user.
        /// </summary>
        [PersonalData]
        [Required]
        [Display(Name = "Straatnaam")]
        public string StreetName { get; set; }

        /// <summary>
        /// Gets or sets the house number for this user.
        /// </summary>
        [PersonalData]
        [Required]
        [Display(Name = "Huisnummer")]
        public int HouseNumber { get; set; }

        /// <summary>
        /// Gets or sets the house number letter for this user.
        /// </summary>
        [PersonalData]
        [Display(Name = "Huisnummer (toevoeging)")]
        public string HouseNumberLetter { get; set; }

        /// <summary>
        /// Gets or sets the postal code for this user.
        /// </summary>
        [PersonalData]
        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        [Display(Name = "Datum indiensttreding")]
        public DateTime DateOfEmployment { get; set; }

        /// <summary>
        /// Gets or sets the IBAN for this user.
        /// </summary>
        [PersonalData]
        [Required]
        [Display(Name = "IBAN")]
        public string IBAN { get; set; }

        /// <summary>
        /// Gets or sets the branch id for this user.
        /// </summary>
        [Required]
        public int BranchId { get; set; }


        /// <summary>
        /// Gets or sets the bumbo id for this user.
        /// </summary>
        [Required]
        public string Bid { get; set; }

        /// <summary>
        /// Gets the fullname of the user.
        /// </summary>
        [Display(Name = "Volledige naam")]
        public string GetFullName()
        {
            return FirstName + " " + LastName;
        }

        public Branch Branch { get; set; }

        public virtual ICollection<ActualTimeWorked> ActualTimeWorked { get; set; }
        public virtual ICollection<AvailableWorktime> AvailableWorktime { get; set; }
        public virtual ICollection<FurloughRequest> FurloughRequest { get; set; }
        public virtual ICollection<PlannedWorktime> PlannedWorktime { get; set; }
        public virtual ICollection<Token> Token { get; set; }
    }
}