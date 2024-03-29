﻿using System.ComponentModel.DataAnnotations;

namespace Bumbo.Web.Models
{
    public class CreateViewModel : UserViewModel
    {
        [DataType(DataType.Password, ErrorMessage = "Wachtwoord voldoet niet aan de eisen")]
        [Required]
        public string Password { get; set; }
    }
}