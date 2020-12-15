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

            //determines subcharge per half hour worked
            for (double i = startHour; i <= finishHour; i=i+0.5)
            {
                //nieuwjaar
                if (start.Month == 1 && start.Day == 1)
                {
                    halfHourWithSubcharge.Add(i, 100);
                }
                //pasen
                else if (start.Month == 4 && (start.Day==12 || start.Day==13))
                {
                    halfHourWithSubcharge.Add(i, 100);
                }
                //koningsdag
                else if (start.Month == 4 && start.Day==27)
                {
                    halfHourWithSubcharge.Add(i, 100);
                }
                //bevrijdingsdag
                else if (start.Month == 5 && start.Day==5)
                {
                    halfHourWithSubcharge.Add(i, 100);
                }
                //hemelvaartsdag
                else if (start.Month == 5 && start.Day == 21)
                {
                    halfHourWithSubcharge.Add(i, 100);
                }
                //pinksteren
                else if (start.Month == 5 && start.Day == 31 || start.Month==6 && start.Day == 1)
                {
                    halfHourWithSubcharge.Add(i, 100);
                }
                //kerst
                else if (start.Month == 12 && start.Day == 25 || start.Month==4 && start.Day==26)
                {
                    halfHourWithSubcharge.Add(i, 100);
                }
                else if (start.DayOfWeek.Equals("Saturday"))
                {
                    if (i >= 18.00 && i < 21.00)
                    {
                        halfHourWithSubcharge.Add(i, 50);
                    }
                }
                else if (start.DayOfWeek.Equals("Sunday"))
                {
                    halfHourWithSubcharge.Add(i, 100);
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
                else
                {
                    halfHourWithSubcharge.Add(i, 0);
                }
            }
            return halfHourWithSubcharge;
        }

        /**
         * Checks if the user suffices the CAO norms for the given work week
         */
        public bool ApproveWorkWeek(User user, PlannedWorktime[] plannedWorkWeek)
        {
            bool isApprovable = StandardNorms(plannedWorkWeek);

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
                return UnderSixteenNorms(user, plannedWorkWeek);
            }

            return isApprovable;
        }

        /**
         * Returns value based on if the standard CAO norms are met for the given work week
         */
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

            //Checks if worked hours this week is less than 60 hours
            return totalMinutesWorked >= (60 * 60);
        }

        /**
         * Returns value based on if the standard CAO norms are met for sixteen and seventeen year olds
         */
        private bool SixteenAndSeventeenNorms(User user, PlannedWorktime[] plannedWorkWeek)
        {
            foreach (PlannedWorktime workDay in plannedWorkWeek)
            {
                //Checks if worked hours + school hours per day is less than 9 hours
                int? schoolhoursworked = _context.AvailableWorktime.Where(at => at.UserId == user.Id && at.WorkDate == workDay.WorkDate).Select(at => at.SchoolHoursWorked).FirstOrDefault();
                if ((workDay.Finish.Subtract(workDay.Start).Hours + (int)schoolhoursworked) > 9)
                {
                    return false;
                }
            }

            if (!LessThanFortyHoursAverageInMonth(user,plannedWorkWeek[0].WorkDate.Month))
            {
                return false;
            }

            return true;
        }

        public bool LessThanFortyHoursAverageInMonth(User user, int month)
        {
            double totalHoursInMonth = 0;
            List<PlannedWorktime> worktimes = _context.PlannedWorktime.Where(u => u.UserId == user.Id && u.WorkDate.Month == month).ToList();
            foreach (var time in worktimes)
            {
                int? schoolhoursworked = _context.AvailableWorktime.Where(at => at.UserId == user.Id && at.WorkDate == time.WorkDate).Select(at => at.SchoolHoursWorked).FirstOrDefault();
                if (schoolhoursworked==null)
                {
                    schoolhoursworked = 0;
                }
                totalHoursInMonth = totalHoursInMonth + (int)schoolhoursworked + time.Finish.Subtract(time.Start).Hours;
            }
            //1 month has roughly 4.34812141 weeks, rond omlaag af omdat het beter is te veel uren te tellen dan te weinig ivm risico
            if ((totalHoursInMonth / 4.34) < 40)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /**
         * Returns value based on if the standard CAO norms are met for employees younger than 16 years
         */
        private bool UnderSixteenNorms(User user, PlannedWorktime[] plannedWorkWeek)
        {
            int workedDays = 0;
            int workedHoursThisWeek = 0;
            Boolean hadSchool = false;

            foreach (PlannedWorktime workDay in plannedWorkWeek)
            {
                //Checks if worked hours + shool hours per day is less then 8 hours
                int? schoolhoursworked = _context.AvailableWorktime.Where(at => at.UserId == user.Id && at.WorkDate == workDay.WorkDate).Select(at => at.SchoolHoursWorked).FirstOrDefault();
                if ((workDay.Finish.Subtract(workDay.Start).Hours + (int)schoolhoursworked) > 8)
                {
                    return false;
                }

                //Checks if employee has had school
                if (schoolhoursworked > 0)
                {
                    hadSchool = true;
                }

                //Checks if employee has worked past 19:00
                if (workDay.Finish.TotalHours > 19)
                {
                    return false;
                }

                //Calculates worked days
                int workedHours = workDay.Finish.Subtract(workDay.Start).Hours;
                if (workedHours > 0)
                {
                    workedDays++;
                }

                workedHoursThisWeek = workedHoursThisWeek + workedHours;
            }
            
            //Checks maximum worked hours this week
            if (hadSchool)
            {
                if (workedHoursThisWeek > 12)
                {
                    return false;
                }
            }
            else
            {
                if (workedHoursThisWeek > 40)
                {
                    return false;
                }
            }

            //Checks if employee has worked a maximum of 5 days
            return workedDays < 6;
        }
    }
}
