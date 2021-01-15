using Bumbo.Data;
using Bumbo.Data.Models;
using Nager.Date;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bumbo.Data.Services
{
    public class CAOService : ICAOService
    {
        private readonly ApplicationDbContext _context;

        public CAOService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// For each half hour worked, calculates the surcharge for the coming half hour
        /// </summary>
        /// <param name="start">Starttime</param>
        /// <param name="finish">Finishtime</param>
        /// <returns>A list of the surcharges per half hour. Note: The surcharge is for the next half hour worked</returns>
        public Dictionary<double, int> WorkdaySurcharge(DateTime start, DateTime finish)
        {
            int workedHours = finish.Subtract(start).Hours;
            Dictionary<double, int> halfHourWithSubcharge = new Dictionary<double, int>(); //tijd in double met minuten als /60 - charge in procent
            double startHour = double.Parse(start.ToString("HH")) + (double.Parse(start.ToString("mm")) / 60);
            double finishHour = double.Parse(finish.ToString("HH")) + (double.Parse(finish.ToString("mm")) / 60);

            /// Checks if dates are valid
            if (!start.DayOfWeek.Equals(finish.DayOfWeek))
            {
                throw new System.ArgumentException("Start and finish times are on different days");
            }
            else if (workedHours <= 0)
            {
                throw new System.ArgumentException("Cannot have worked 0 hours or less");
            }
            else if (start > finish || finish < start)
            {
                throw new System.ArgumentException("Start time cannot be more than Finish Time, and vice versa");
            }

            /// Determines subcharge per half hour worked
            for (double i = startHour; i < finishHour; i = i + 0.5)
            {
                ///Holiday check
                if (DateSystem.IsPublicHoliday(start, CountryCode.NL))
                {
                    halfHourWithSubcharge.Add(i, 100);
                }
                else if (start.DayOfWeek.ToString().Equals("Saturday") && (i >= 18.00 && i < 24.00))
                {
                    halfHourWithSubcharge.Add(i, 50);
                }
                else if (start.DayOfWeek.ToString().Equals("Sunday"))
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

        /// <summary>
        /// Checks if the user suffices the CAO norms for the given work week
        /// </summary>
        /// <param name="user">Specified user which needs approvement</param>
        /// <param name="plannedWorkWeek">The workweek which needs to be approved</param>
        /// <returns>A boolean which indicates whether the given workweek is approved</returns>
        public List<string> WorkWeekValidate(User user, PlannedWorktime[] plannedWorkWeek)
        {
            List<string> validationErrors = StandardNorms(plannedWorkWeek, user);

            int userage = (int)((DateTime.Today - user.DateOfBirth).TotalDays / 365);
            if (userage == 16 || userage == 17)
            {
                List<string> validationErrorsSixteenAndSeventeen = SixteenAndSeventeenNorms(user, plannedWorkWeek);
                if (validationErrorsSixteenAndSeventeen != null)
                {
                    foreach (var error in validationErrorsSixteenAndSeventeen)
                    {
                        validationErrors.Add(error);
                    }
                }
            }
            else if (userage < 16)
            {
                List<string> validationErrorsUnder16 = UnderSixteenNorms(user, plannedWorkWeek);
                if (validationErrorsUnder16 != null)
                {
                    foreach (var error in validationErrorsUnder16)
                    {
                        validationErrors.Add(error);
                    }
                }
            }
            return validationErrors;
        }

        public List<string> StandardNorms(PlannedWorktime[] plannedWorkWeek, User user)
        {
            List<string> validationErrors = new List<string>();
            double totalMinutesWorked = 0;
            foreach (PlannedWorktime workDay in plannedWorkWeek)
            {
                /// Checks if worked hours per day is less then 12 hours
                double workedMinutes = workDay.Start.Subtract(workDay.Finish).TotalMinutes;
                if (workedMinutes > (12 * 60))
                {
                    validationErrors.Add(user.GetFullName() + " heeft op: " + workDay.WorkDate.ToShortDateString() + " meer dan 12 uur gepland staan");
                }
                else
                {
                    totalMinutesWorked = totalMinutesWorked + workedMinutes;
                }
            }

            /// Checks if worked hours this week is less than 60 hours
            if (totalMinutesWorked >= (60 * 60))
            {
                validationErrors.Add(user.GetFullName() + " heeft meer dan 60 uur gepland staan in deze week");
            }
            
            return validationErrors;
        }

        public List<string> SixteenAndSeventeenNorms(User user, PlannedWorktime[] plannedWorkWeek)
        {
            List<string> validationErrors = new List<string>();
            foreach (PlannedWorktime workDay in plannedWorkWeek)
            {
                /// Checks if worked hours + school hours per day is less than 9 hours
                int? schoolhoursworked = _context.AvailableWorktime.Where(at => at.UserId == user.Id && at.WorkDate == workDay.WorkDate).Select(at => at.SchoolHoursWorked).FirstOrDefault();
                if ((workDay.Finish.Subtract(workDay.Start).Hours + (int)schoolhoursworked) > 9)
                {
                    validationErrors.Add("Minderjarige van 16-17 jaar oud: " + user.GetFullName() + " heeft meer dan 9 uur gepland staan op: " + workDay.WorkDate.ToShortDateString());
                }
            }

            var result = LessThanFortyHoursAverageInMonth(user, plannedWorkWeek[0].WorkDate.Month);

            if (result != null) validationErrors.Add(result);
            
            return validationErrors;
        }

        public string LessThanFortyHoursAverageInMonth(User user, int month)
        {
            double totalHoursInMonth = 0;
            List<PlannedWorktime> worktimes = _context.PlannedWorktime.Where(u => u.UserId == user.Id && u.WorkDate.Month == month).ToList();
            foreach (var time in worktimes)
            {
                int? schoolhoursworked = _context.AvailableWorktime.Where(at => at.UserId == user.Id && at.WorkDate == time.WorkDate).Select(at => at.SchoolHoursWorked).FirstOrDefault();
                if (schoolhoursworked == null)
                {
                    schoolhoursworked = 0;
                }
                totalHoursInMonth = totalHoursInMonth + (int)schoolhoursworked + time.Finish.Subtract(time.Start).Hours;
            }
            /// 1 month has roughly 4.34812141 weeks, rounded downwords to minimize risk of overpaying
            if ((totalHoursInMonth / 4.34) < 40)
            {
                return "Minderjarige van 16-17 jaar oud: " + user.GetFullName() + " heeft meer dan 40 uur gemiddeld in maandnummer: " + month + " gepland staan.";
            }
            return null;
        }

        public List<string> UnderSixteenNorms(User user, PlannedWorktime[] plannedWorkWeek)
        {
            int workedDays = 0;
            int workedHoursThisWeek = 0;
            Boolean hadSchool = false;
            List<string> validationErrors = new List<string>();

            foreach (PlannedWorktime workDay in plannedWorkWeek)
            {
                var schoolhoursworked = _context.AvailableWorktime.Where(at => at.UserId == user.Id && at.WorkDate == workDay.WorkDate).Select(at => at.SchoolHoursWorked).FirstOrDefault();
               
                if (schoolhoursworked != null)
                {
                    if ((workDay.Finish.Subtract(workDay.Start).Hours + (int)schoolhoursworked) > 8)
                    {
                        validationErrors.Add(user.GetFullName() + " heeft meer dan 8 uur gepland staan op: " + workDay.WorkDate.ToShortDateString() + ".");
                    }

                    if (schoolhoursworked > 0)
                    {
                        hadSchool = true;
                    }
                }

                if (workDay.Finish.TotalHours > 19)
                {
                    validationErrors.Add(user.GetFullName() + " is ingepland na 19:00.");
                }

                int workedHours = workDay.Finish.Subtract(workDay.Start).Hours;
                if (workedHours > 0)
                {
                    workedDays++;
                }

                workedHoursThisWeek = workedHoursThisWeek + workedHours;
            }

            if (hadSchool)
            {
                if (workedHoursThisWeek > 12)
                {
                    validationErrors.Add(user.GetFullName() + " is meer dan 12 uur ingepland in zijn/haar schoolweek.");
                }
            }
            else
            {
                if (workedHoursThisWeek > 40)
                {
                    validationErrors.Add(user.GetFullName() + " is meer dan 40 uur ingepland deze week.");
                }
            }

            if (workedDays >= 6)
            {
                validationErrors.Add(user.GetFullName() + " is meer dan 5 dagen ingepland in deze week.");
            }

            return validationErrors;
        }
    }
}
