using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Bumbo.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Bumbo.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Bumbo.Web.Models;
using System;
using Bumbo.Data.Services;

namespace Bumbo.Web.Controllers
{
    [Authorize]
    public class ScheduleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ICAOService _caoService;

        public ScheduleController(ApplicationDbContext context, UserManager<User> userManager, ICAOService caoService)
        {
            _context = context;
            _userManager = userManager;
            _caoService = caoService;
        }

        public async Task<IActionResult> IndexAsync(ScheduleViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            var newModel = new ScheduleViewModel
            {
                UserId = model.UserId != 0 ? model.UserId : user.Id,
                Users = User.IsInRole("Manager") ? await _context.Users.Where(u => u.BranchId == user.BranchId).ToListAsync() : null
            };

            return View(newModel);
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Plan(SchedulePlanViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var users = await _context.Users.Where(u => u.BranchId == user.BranchId).ToListAsync();
            var date = model.Date >= DateTime.Today ? model.Date : GetBeginningOfWeek(DateTime.Today);
            var errors = await CalculateMonthlyCAOAsync(users, date);
            
            var newModel = new SchedulePlanViewModel
            {
                Users = users,
                Date = date,
                Errors = errors
            };

            return View(newModel);
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Create(int userId, DateTime date)
        {
            if (userId < 1 || date == null) return View("Plan");

            var user = await _context.Users.FindAsync(userId);
            var beginningOfWeek = GetBeginningOfWeek(date);
            var endOfWeek = beginningOfWeek.AddDays(7);

            var availableTimes = await _context.AvailableWorktime
                .Where(t => t.UserId == user.Id)
                .Where(t => t.WorkDate >= beginningOfWeek)
                .Where(t => t.WorkDate <= endOfWeek)
                .ToListAsync();

            var plannedWorktimes = await _context.PlannedWorktime
                .Where(t => t.UserId == user.Id)
                .Where(t => t.WorkDate >= beginningOfWeek)
                .Where(t => t.WorkDate <= endOfWeek)
                .ToListAsync();

            var prognoses = await _context.Prognoses
                .Where(p => p.Date >= beginningOfWeek)
                .Where(p => p.Date <= endOfWeek)
                .ToListAsync();

            var model = new ScheduleCreateViewModel
            {
                AvailableWorkTimes = new List<AvailableWorktime>(),
                PlannedWorktimes = new List<PlannedWorktime>(),
                Prognoses = new List<Prognoses>(),
                MinimumDate = beginningOfWeek,
                MaximumDate = endOfWeek,
                UserName = user.GetFullName(),
                UserId = userId,
                Errors = _caoService.WorkWeekValidate(user, plannedWorktimes.ToArray())
            };

            for (int i = 0; i < 7; i++)
            {
                var day = beginningOfWeek.AddDays(i);

                model.AvailableWorkTimes.Add(availableTimes.Find(t => t.WorkDate == day));
                model.PlannedWorktimes.Add(plannedWorktimes.Find(p => p.WorkDate == day));
                model.Prognoses.Add(prognoses.Find(p => p.Date == day));
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Store(ScheduleCreateViewModel model)
        {
            var user = await _context.Users.FindAsync(model.UserId);
            model.Errors = _caoService.WorkWeekValidate(user, model.PlannedWorktimes.ToArray());

            foreach (var plannedWorktime in model.PlannedWorktimes)
            {
                var exists = await _context.PlannedWorktime
                    .AsNoTracking()
                    .Where(p => p.UserId == plannedWorktime.UserId)
                    .Where(p => p.WorkDate == plannedWorktime.WorkDate)
                    .AnyAsync();

                if (plannedWorktime.Start.TotalHours == 0 && plannedWorktime.Finish.TotalHours == 0)
                {
                    if (exists) _context.PlannedWorktime.Remove(plannedWorktime);
                    continue;
                }

                if (plannedWorktime.Start.TotalHours == 0 || plannedWorktime.Finish.TotalHours == 0)
                {
                    model.Errors.Add($"{plannedWorktime.WorkDate.ToShortDateString()} - Een van de tijden mist!");
                    break;
                }

                if (plannedWorktime.Start >= plannedWorktime.Finish)
                {
                    model.Errors.Add($"{plannedWorktime.WorkDate.ToShortDateString()} - Startijd kan niet hoger zijn dan eindtijd!");
                    break;
                }

                if (
                    plannedWorktime.Start.Hours < 6 ||
                    plannedWorktime.Finish.Hours > 23 ||
                    plannedWorktime.Start.Hours > 23 ||
                    plannedWorktime.Finish.Hours < 6
                )
                {
                    model.Errors.Add($"{plannedWorktime.WorkDate.ToShortDateString()} - Tijden mogen niet buiten 06:00 en 23:00 vallen!");
                    break;
                }              

                if (exists) _context.PlannedWorktime.Update(plannedWorktime);
                else await _context.PlannedWorktime.AddAsync(plannedWorktime);
            }

            await _context.SaveChangesAsync();

            if (model.Errors.Count > 0) return await ShowErrorsAsync(model);

            return RedirectToAction("Plan");
        }

        private async Task<IActionResult> ShowErrorsAsync(ScheduleCreateViewModel model)
        {
            var beginningOfWeek = GetBeginningOfWeek(model.MinimumDate);
            var endOfWeek = beginningOfWeek.AddDays(7);

            var availableWorktimes = await _context.AvailableWorktime
                .Where(t => t.UserId == model.UserId)
                .Where(t => t.WorkDate >= beginningOfWeek)
                .Where(t => t.WorkDate <= endOfWeek)
                .ToListAsync();

            var prognoses = await _context.Prognoses
                .Where(p => p.Date >= beginningOfWeek)
                .Where(p => p.Date <= endOfWeek)
                .ToListAsync();

            model.AvailableWorkTimes = new List<AvailableWorktime>();
            model.Prognoses = new List<Prognoses>();

            for (int i = 0; i < 7; i++)
            {
                var day = beginningOfWeek.AddDays(i);

                model.AvailableWorkTimes.Add(availableWorktimes.Find(t => t.WorkDate == day));
                model.Prognoses.Add(prognoses.Find(p => p.Date == day));
            }

            return View("Create", model);
        }

        private DateTime GetBeginningOfWeek(DateTime date)
        {
            return date.AddDays(-(int)date.DayOfWeek + (int)DayOfWeek.Monday);
        }

        private async Task<List<string>> CalculateMonthlyCAOAsync(List<User> users, DateTime date)
        {
            var errors = new List<string>();

            foreach (var user in users)
            {
                var plannedWorktimes = await _context.PlannedWorktime
                    .Where(t => t.UserId == user.Id)
                    .Where(t => t.WorkDate.Month == date.Month)
                    .ToListAsync();

                errors.AddRange(_caoService.WorkWeekValidate(user, plannedWorktimes.ToArray()));
            }

            return errors;
        }

        [HttpGet]
        public async Task<ActionResult<List<ScheduleResponseViewModel>>> GetPlannedWorkTime(DateTime start, DateTime end, int? id)
        {
            if (id == null || id == 0)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                id = user.Id;
            }

            var plannedWorktimes = await _context.PlannedWorktime
                .Where(p => p.WorkDate >= start)
                .Where(p => p.WorkDate <= end)
                .Where(p => p.UserId == id)
                .ToListAsync();

            var times = new List<ScheduleResponseViewModel>();

            foreach (var time in plannedWorktimes)
            {
                times.Add(new ScheduleResponseViewModel
                {
                    Title = $"Werken - {time.Section}",
                    Start = time.WorkDate.AddHours(time.Start.TotalHours),
                    End = time.WorkDate.AddHours(time.Finish.TotalHours)
                });
            }

            return times;
        }
    }
}
