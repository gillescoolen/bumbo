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

namespace Bumbo.Web.Controllers
{
    [Authorize]
    public class ScheduleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ScheduleController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
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
            var date = model.Date >= DateTime.Today ? model.Date : getBeginningOfWeek(DateTime.Today);

            var newModel = new SchedulePlanViewModel
            {
                Users = users,
                Date = date
            };

            return View(newModel);
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Create(int userId, DateTime date)
        {
            if (userId < 1 || date == null) return View("Plan");

            var user = await _context.Users.FindAsync(userId);
            var beginningOfWeek = getBeginningOfWeek(date);
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
                User = user,
                AvailableWorkTimes = new List<AvailableWorktime>(),
                PlannedWorktimes = new List<PlannedWorktime>(),
                Prognoses = new List<Prognoses>(),
                MinimumDate = beginningOfWeek,
                MaximumDate = endOfWeek
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
            var errors = new List<string>();

            foreach (var plannedWorktime in model.PlannedWorktimes)
            {
                var exists = await _context.PlannedWorktime
                    .AsNoTracking()
                    .Where(p => p.UserId == plannedWorktime.UserId)
                    .Where(p => p.WorkDate == plannedWorktime.WorkDate)
                    .AnyAsync();

                if (exists) _context.PlannedWorktime.Update(plannedWorktime);
                else
                {
                    
                    await _context.PlannedWorktime.AddAsync(plannedWorktime);
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Plan");
        }

        private DateTime getBeginningOfWeek(DateTime date)
        {
            return date.AddDays(-(int)date.DayOfWeek + (int)DayOfWeek.Monday);
        }
    }
}
