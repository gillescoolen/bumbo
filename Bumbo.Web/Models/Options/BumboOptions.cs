using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bumbo.Web.Models.Options
{
    public class BumboOptions
    {
        public const string Bumbo = "Bumbo";

        public bool RegistrationEnabled { get; set; } = false;
        public bool TwoFactorAuthenticationEnabled { get; set; } = false;
        public bool EmailIsManageable { get; set; } = false;
    }
}
