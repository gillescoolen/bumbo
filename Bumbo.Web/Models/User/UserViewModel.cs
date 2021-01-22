using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Bumbo.Data.Models;

namespace Bumbo.Web.Models.User
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public Branch Branch { get; set; }

        [Display(Name = "Telefoonnummer")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

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
        [RegularExpression("[0-9]{4}[A-Z]{2}", ErrorMessage = "The Postcode field does not meet the correct format  (1234 AZ)")]
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
        [Range(0, Int32.MaxValue)]
        public int HouseNumber { get; set; }

        /// <summary>
        /// Gets or sets the house number letter for this user.
        /// </summary>
        [Display(Name = "Toevoeging")]
        [RegularExpression("[A-Z]", ErrorMessage = "The Toevoeging field does not meet the correct format (A-Z)")]
        [StringLength(1)]
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

        [DataType(DataType.EmailAddress, ErrorMessage = "Email voldoet niet aan de eisen")]
        [Required]
        public string Email { get; set; }

        [Display(Name = "Rol")]
        [Required]
        public Roles Role { get; set; }

        public enum Roles
        {
            Manager = 1,
            User = 0
        }
    }
}
