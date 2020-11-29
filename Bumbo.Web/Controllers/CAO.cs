using Bumbo.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bumbo.Web.Controllers
{
    public class CAO
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public CAO (ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public int WorkdaySurcharge (DateTime start, DateTime finish)
        {
            int workedHours = finish.Subtract(start).Hours;

            //Checks if dates are valid
            if (!start.DayOfWeek.Equals(finish.DayOfWeek))
            {
                throw new System.ArgumentException("Start and finish times are on different days");
            } 
            else if (workedHours < 0)
            {
                throw new System.ArgumentException("Cannot have worked less than 0 hours");
            }

            //Determines surcharge
            if (start.DayOfWeek.Equals("Sunday"))
            {
                return 100;
            }

            //TODO Feestdagen
            int startHour = int.Parse(DateTime.Now.ToString("HH"));
            int finishHour = int.Parse(DateTime.Now.ToString("HH"));

            if (start.DayOfWeek.Equals("Saturday"))
            {
                if ((startHour > 18 && startHour < 24) && (finishHour > 18 && finishHour < 24))
                {
                    return 50;
                }
            }
            else if ((startHour > 21 && startHour < 24) && (finishHour > 21 && finishHour < 24))
            {
                return 50;
            } 
            else if ((startHour > 20 && startHour < 24) && (finishHour > 20 && finishHour < 24))
            {
                return 33;
            }
            else if ((startHour > 0 && startHour < 6) && (finishHour > 0 && finishHour < 6))
            {
                return 50;
            }

            return 0;
        }
    }
}
