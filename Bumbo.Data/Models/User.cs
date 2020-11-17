using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Bumbo.Data
{
    public class User : IdentityUser<int>
    {
        /// <summary>
        /// Gets or sets the first name for this user.
        /// </summary>
        [PersonalData]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name for this user.
        /// </summary>
        [PersonalData]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the date of birth for this user.
        /// </summary>
        [PersonalData]
        [DataType(DataType.Date)]
        public string DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the postal code for this user.
        /// </summary>
        [PersonalData]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the house number for this user.
        /// </summary>
        [PersonalData]
        public int? HouseNumber { get; set; }

        /// <summary>
        /// Gets or sets the house number letter for this user.
        /// </summary>
        [PersonalData]
        public string HouseNumberLetter { get; set; }

        /// <summary>
        /// Gets or sets the postal code for this user.
        /// </summary>
        [PersonalData]
        [DataType(DataType.Date)]
        public string DateOfEmployment { get; set; }

        /// <summary>
        /// Gets or sets the IBAN for this user.
        /// </summary>
        [PersonalData]
        public string IBAN { get; set; }

        /// <summary>
        /// Gets or sets the branch id for this user.
        /// </summary>
        [Required]
        public int BranchId { get; set; }

        /// <summary>
        /// Gets or sets the branch id for this user.
        /// </summary>
        public string GetFullName()
        {
            return FirstName + " " + LastName;
        }
    }
}