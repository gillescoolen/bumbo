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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Plan()
        {
            var user = _userManager.GetUserAsync(User).Result;

            var users = _context.Users.ToList().Where(u => u.BranchId == user.BranchId);

            var model = new PlanViewModel
            {
                Users = users,
                Date = DateTime.Now
            };

            return View(model);
        }

        public async Task<IActionResult> Create(int userId, int increment = 0)
        {
            var beginningOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);

            if (increment == 0) beginningOfWeek.AddDays(7 * increment);

            var endOfWeek = beginningOfWeek.AddDays(7);

            var user = await _context.Users.FindAsync(userId);

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

            var model = new CreateViewModel
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
        public async Task<IActionResult> Store(CreateViewModel model)
        {
            foreach (var plannedWorktime in model.PlannedWorktimes)
            {
                var exists = await _context.PlannedWorktime
                    .AsNoTracking()
                    .Where(p => p.UserId == plannedWorktime.UserId)
                    .Where(p => p.WorkDate == plannedWorktime.WorkDate)
                    .FirstAsync();

                if (exists == null) await _context.PlannedWorktime.AddAsync(plannedWorktime);
                else  _context.PlannedWorktime.Update(plannedWorktime);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Plan");
        }
    }
}
