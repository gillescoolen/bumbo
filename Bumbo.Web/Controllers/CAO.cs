using Bumbo.Data;
using Bumbo.Data.Models;
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

        public CAO (ApplicationDbContext context)
        {
            _context = context;
        }

        public CAO ()
        {
            _context = null;
        }

        public Dictionary<double,int> WorkdaySurcharge(DateTime start, DateTime finish)
        {
            int workedHours = finish.Subtract(start).Hours;
            Dictionary<double, int> halfHourWithSubcharge = new Dictionary<double,int>(); //tijd in double met minuten als /60 - charge in procent
            double startHour = double.Parse(start.ToString("HH")) + (double.Parse(start.ToString("mm")) / 60);
            double finishHour = double.Parse(finish.ToString("HH")) + (double.Parse(start.ToString("mm")) / 60);

            //Checks if dates are valid
            if (!start.DayOfWeek.Equals(finish.DayOfWeek))
            {
                throw new System.ArgumentException("Start and finish times are on different days");
            } 
            else if (workedHours <= 0)
            {
                throw new System.ArgumentException("Cannot have worked 0 hours or less");
            }
            else if (start>finish || finish < start)
            {
                throw new System.ArgumentException("Start time cannot be more than Finish Time, and vice versa");
            }

            if (start.DayOfWeek.Equals("Sunday"))
            {
                for (double i = startHour; i < finishHour; i = i + 0.5)
                {
                    halfHourWithSubcharge.Add(i, 100);
                }
                return halfHourWithSubcharge;
            } 

            //determines subcharge per half hour worked
            for (double i = startHour; i <= finishHour; i=i+0.5)
            {
                if (start.DayOfWeek.Equals("Saturday"))
                {
                    if (i >= 18.00 && i < 21.00)
                    {
                        halfHourWithSubcharge.Add(i, 50);
                    }
                }
                else if (i >= 21.00 && i <= 24.00)
                {
                    halfHourWithSubcharge.Add(i, 50);
                }
                else if (i >= 20.00 && i < 21.00)
                {
                    halfHourWithSubcharge.Add(i, 33);
                }
                else if (i >= 0 && i <= 6.00)
                {
                    halfHourWithSubcharge.Add(i, 50);
                }
            }

            //TODO Feestdagen (per feestdatum kijken of date erop valt)
            
            return halfHourWithSubcharge;
        }

        public Boolean ApproveWorkWeek(User user, PlannedWorktime[] plannedWorkWeek)
        {
            Boolean isApprovable = StandardNorms(plannedWorkWeek);

            if (!isApprovable)
            {
                return isApprovable;
            }

            int userage = (int)((DateTime.Today - user.DateOfBirth).TotalDays / 365);
            if (userage == 16 || userage == 17)
            {
                return SixteenAndSeventeenNorms(user, plannedWorkWeek);
            } 
            else if (userage < 16)
            {
                return UnderSixteenNorms(plannedWorkWeek);
            }

            return isApprovable;
        }

        private Boolean StandardNorms(PlannedWorktime[] plannedWorkWeek)
        {
            double totalMinutesWorked = 0;
            foreach (PlannedWorktime workDay in plannedWorkWeek)
            {
                //Checks if worked hours per day is less then 12 hours
                double workedMinutes = workDay.Start.Subtract(workDay.Finish).TotalMinutes;
                if (workedMinutes > (12 * 60))
                {
                    return false;
                } 
                else
                {
                    totalMinutesWorked = totalMinutesWorked + workedMinutes;
                }
            }

            //Checks if worked hours this week is less then 60 hours
            return totalMinutesWorked >= (60 * 60);
        }

        private bool SixteenAndSeventeenNorms(User user, PlannedWorktime[] plannedWorkWeek)
        {
            int userage = (int)((DateTime.Today - user.DateOfBirth).TotalDays / 365);
            if (userage < 18)
            {
                for (int i = 0; i < plannedWorkWeek.Length; i++)
                {
                    int? schoolhoursworked = _context.AvailableWorktime.Where(at => at.UserId == user.Id && at.WorkDate == plannedWorkWeek[i].WorkDate).Select(at => at.SchoolHoursWorked).FirstOrDefault();
                    if (schoolhoursworked!=null)
                    {
                        if (userage == 16 || userage == 17)
                        {
                            if (plannedWorkWeek[i].Finish.Subtract(plannedWorkWeek[i].Start).Hours + (int)schoolhoursworked > 9)
                            {
                                return false;
                            }
                        }
                        else if (userage < 16)
                        {
                            if (plannedWorkWeek[i].Finish.Subtract(plannedWorkWeek[i].Start).Hours + (int)schoolhoursworked > 8)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        private bool UnderSixteenNorms(PlannedWorktime[] plannedWorkWeek)
        {
                foreach (var item in plannedWorkWeek)
                {
                    if (item.Finish.Hours > 19)
                    {
                        return false;
                    }
                }
            return true;
        }
    }
}
