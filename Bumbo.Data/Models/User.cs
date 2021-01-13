using Bumbo.Data;
using Bumbo.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Threading.Tasks;

namespace Bumbo.Data.Models
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
        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        [Display(Name = "Voornaam")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name for this user.
        /// </summary>
        [PersonalData]
        [Required]
        [Column(TypeName = "varchar(50)")]
        [StringLength(50)]
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
        [StringLength(10)]
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the house number for this user.
        /// </summary>
        [PersonalData]
        [Required]
        [Column(TypeName = "varchar(50)")]
        [Display(Name = "Straatnaam")]
        [StringLength(50)]
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
        [Column(TypeName = "varchar(10)")]
        [Display(Name = "Toevoeging")]
        [StringLength(10)]
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
        [Column(TypeName = "varchar(50)")]
        [Display(Name = "IBAN")]
        [StringLength(50)]
        public string IBAN { get; set; }

        /// <summary>
        /// Gets or sets the branch id for this user.
        /// </summary>
        [Required]
        [Display(Name = "Filiaal")]
        public int BranchId { get; set; }


        /// <summary>
        /// Gets or sets the bumbo id for this user.
        /// </summary>
        [Required]
        [Column(TypeName = "varchar(36)")]
        [StringLength(36)]
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