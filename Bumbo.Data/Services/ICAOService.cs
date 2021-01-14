
using System;
using System.Collections.Generic;
using Bumbo.Data.Models;

namespace Bumbo.Data.Services
{
    public interface ICAOService
    {
        string LessThanFortyHoursAverageInMonth(User user, int month);
        List<string> SixteenAndSeventeenNorms(User user, PlannedWorktime[] plannedWorkWeek);
        List<string> StandardNorms(PlannedWorktime[] plannedWorkWeek);
        List<string> UnderSixteenNorms(User user, PlannedWorktime[] plannedWorkWeek);
        Dictionary<double, int> WorkdaySurcharge(DateTime start, DateTime finish);
        List<string> WorkWeekValidate(User user, PlannedWorktime[] plannedWorkWeek);
    }
}
