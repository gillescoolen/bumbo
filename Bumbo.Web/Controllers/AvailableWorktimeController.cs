using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bumbo.Data;
using Bumbo.Data.Models;
using Microsoft.AspNetCore.Identity;
using Bumbo.Web.Models;
using System.Web;

namespace Bumbo.Web.Controllers
{
    public class AvailableWorktimeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        public AvailableWorktimeController(ApplicationDbContext context, UserManager<User> user)
        {
            _context = context;
            _userManager = user;
        }

        public async Task<IActionResult> Index()
        {
            var user = _userManager.GetUserAsync(User).Result;
            ViewBag.UserAge = (int)((DateTime.Today - user.DateOfBirth).TotalDays / 365);
            //Als rol = niet bevoegd alle users te zien => ziet alleen eigen available worktime
            if (User.IsInRole("Admin"))
            {
                var applicationDbContext = _context.AvailableWorktime.Include(a => a.User);
                return View(await applicationDbContext.ToListAsync());
            }
            else
            {
                var applicationDbContext = _context.AvailableWorktime.Include(a => a.User).Where(a => a.UserId == user.Id);
                return View(await applicationDbContext.ToListAsync());
            }
        }

        public IActionResult Create()
        {
            var user = _userManager.GetUserAsync(User).Result;
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Bid");
            DateTime maxDate = DateTime.Today;
            ViewBag.UserAge = (int)((maxDate - user.DateOfBirth).TotalDays / 365);
            AvailableWorktime lastFilledWorkTime = _context.AvailableWorktime.Where(wt => wt.UserId == user.Id).OrderByDescending(p => p.WorkDate).FirstOrDefault();

            //Bepaalt welke volgende dates er komen te staan die moeten worden ingevuld
            List<DateTime> newWeek = new List<DateTime>();
            if (lastFilledWorkTime == null)
            {
                for (int i = 1; i < 9; i++)
                {
                    newWeek.Add(maxDate.AddDays(i));
                }
            }
            else if (maxDate >= lastFilledWorkTime.WorkDate)
            {
                for (int i = 1; i < 9; i++)
                {
                    newWeek.Add(maxDate.AddDays(i));
                }
            }
            else
            {
                for (int i = 1; i < 9; i++)
                {
                    newWeek.Add(lastFilledWorkTime.WorkDate.AddDays(i));
                }
            }

            ViewBag.newDates = newWeek;
            return View();
        }

        //Maakt date voor komende 8 dagen
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AvailableWorkTimeViewModel model)
        {
            var user = _userManager.GetUserAsync(User).Result;
            for (int index = 0; index < model.Start.Count; index++)
            {
                AvailableWorktime availableWorktime = new AvailableWorktime
                {
                    UserId = user.Id,
                    WorkDate = model.Dates[index],
                    SchoolHoursWorked = model.SchoolHoursWorked,
                    Start = model.Start[index],
                    Finish = model.Finish[index]
                };

                _context.Add(availableWorktime);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        //Werkt nog niet ivm workdate
        [HttpGet("Edit/{UserId}/{WorkDate}")]
        public async Task<IActionResult> Edit(int? UserId, string? WorkDate)
        {
            if (UserId == null || WorkDate == null)
            {
                return NotFound();
            }
            var user = _userManager.GetUserAsync(User).Result;
            ViewBag.UserAge = (int)((DateTime.Now - user.DateOfBirth).TotalDays / 365);

            var decoded = HttpUtility.UrlDecode(WorkDate);
            DateTime date = DateTime.Parse(decoded);
            var availableWorktime = await _context.AvailableWorktime.Where(at => at.UserId == UserId && at.WorkDate == date).FirstOrDefaultAsync();
            if (availableWorktime == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Bid", availableWorktime.UserId);
            return View(availableWorktime);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditConfirmed(int userId, AvailableWorktime availableWorktime)
        {
            if (userId != availableWorktime.UserId)
            {
                return NotFound();
            }
            var toBeUpdated = _context.AvailableWorktime.Where(a => a.UserId == userId && a.WorkDate == availableWorktime.WorkDate).FirstOrDefault();
            try
            {
                if (ModelState.IsValid)
                {
                    toBeUpdated.Start = availableWorktime.Start;
                    toBeUpdated.Finish = availableWorktime.Finish;
                    toBeUpdated.SchoolHoursWorked = availableWorktime.SchoolHoursWorked;
                    _context.Update(toBeUpdated);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AvailableWorktimeExists(availableWorktime.WorkDate))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Delete/{UserId}/{WorkDate}")]
        public async Task<IActionResult> Delete(int? UserId, string? WorkDate)
        {
            if (UserId == null || WorkDate == null)
            {
                return NotFound();
            }

            var decoded = HttpUtility.UrlDecode(WorkDate);
            DateTime date = DateTime.Parse(decoded);
            var availableWorktime = await _context.AvailableWorktime
                .Include(a => a.User)
                .FirstOrDefaultAsync(at => at.UserId == UserId && at.WorkDate == date);

            if (availableWorktime == null)
            {
                return NotFound();
            }

            return View(availableWorktime);
        }

        [HttpGet("DeleteConfirmed/{UserId}/{WorkDate}")]
        public async Task<IActionResult> DeleteConfirmed(int UserId, string WorkDate)
        {
            DateTime workDate = DateTime.Parse(HttpUtility.UrlDecode(WorkDate));
            var availableWorktime = await _context.AvailableWorktime.Where(at => at.UserId == UserId && at.WorkDate == workDate).FirstOrDefaultAsync();
            _context.AvailableWorktime.Remove(availableWorktime);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AvailableWorktimeExists(DateTime id)
        {
            return _context.AvailableWorktime.Any(e => e.WorkDate == id);
        }
    }
}
