using Bumbo.Data;
using Bumbo.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bumbo.Web.Models
{
    public class DashboardViewModel
    {
        /// <summary>
        /// Returns greeting relevant to the time of the day
        /// </summary>
        public string GetGreeting()
        {
            DateTime dateTime = DateTime.Now;

            if (dateTime.Hour >= 5 && dateTime.Hour < 12) return "Goedemorgen";
            else if (dateTime.Hour >= 12 && dateTime.Hour < 16) return "Goedemiddag";
            else return "Goedenavond";
        }
    }
}
