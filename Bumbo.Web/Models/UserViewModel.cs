using Bumbo.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Bumbo.Web.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public Branch Branch { get; set; }

        [Display(Name = "Telefoonnummer")]
        public int? PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the first name for this user.
        /// </summary>
        [Required]
        [StringLength(50)]
        [Display(Name = "Voornaam")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name for this user.
        /// </summary>
        [Required]
        [StringLength(50)]
        [Display(Name = "Achternaam")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the date of birth for this user.
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Geboortedatum")]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the postal code for this user.
        /// </summary>
        [Required]
        [DataType(DataType.PostalCode)]
        [Display(Name = "Postcode")]
        [StringLength(10)]
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the house number for this user.
        /// </summary>
        [Required]
        [Display(Name = "Straatnaam")]
        [StringLength(50)]
        public string StreetName { get; set; }

        /// <summary>
        /// Gets or sets the house number for this user.
        /// </summary>
        [Required]
        [Display(Name = "Huisnummer")]
        public int HouseNumber { get; set; }

        /// <summary>
        /// Gets or sets the house number letter for this user.
        /// </summary>
        [Display(Name = "Toevoeging")]
        [StringLength(10)]
        public string HouseNumberLetter { get; set; }

        /// <summary>
        /// Gets or sets the postal code for this user.
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Datum indiensttreding")]
        public DateTime DateOfEmployment { get; set; }

        /// <summary>
        /// Gets or sets the IBAN for this user.
        /// </summary>
        [Required]
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

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        public Roles Role { get; set; }

        public enum Roles
        {
            Manager = 1,
            User = 0
        }
    }
}
