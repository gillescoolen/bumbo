#nullable enable
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
using System.Globalization;

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

        [HttpGet("AvailableWorktime/{order}")]
        public async Task<IActionResult> Index(string? order)
        {
            var user = _userManager.GetUserAsync(User).Result;
            ViewBag.UserAge = (int)((DateTime.Today - user.DateOfBirth).TotalDays / 365);
            CultureInfo dutchculture = new CultureInfo("nl-NL");
            ViewBag.cultureinfo = dutchculture;
            ViewBag.order = order;

            var availableWorkTime = _context.AvailableWorktime.Include(a => a.User).Where(a => a.UserId == user.Id && a.User.BranchId == user.BranchId); ;

            if (User.IsInRole("Manager"))
            {
                availableWorkTime = _context.AvailableWorktime.Include(a => a.User);
            }

            if (order != null && order.Equals("Standard"))
            {
                availableWorkTime = availableWorkTime.OrderBy(a => a.UserId);
            }
            else if (order.Equals("Werkdag"))
            {
                availableWorkTime = availableWorkTime.OrderBy(a => a.WorkDate);
            }
            else if (order.Equals("Starttijd"))
            {
                availableWorkTime = availableWorkTime.OrderBy(a => a.Start);
            }
            else if (order.Equals("Eindtijd"))
            {
                availableWorkTime = availableWorkTime.OrderBy(a => a.Finish);
            }
            else if (order.Equals("Medewerker"))
            {
                availableWorkTime = availableWorkTime.OrderBy(a => a.User.FirstName);
            }
            else if (order.Equals("Schooluren"))
            {
                availableWorkTime = availableWorkTime.OrderBy(a => a.SchoolHoursWorked);
            }

            return View(await availableWorkTime.ToListAsync());
        }

        public IActionResult Create()
        {
            if (User.IsInRole("Manager"))
            {
                return RedirectToAction("Standard", "AvailableWorktime");
            }

            CultureInfo dutchculture = new CultureInfo("nl-NL");
            ViewBag.cultureinfo = dutchculture;
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
            if (User.IsInRole("Manager"))
            {
                return RedirectToAction("Standard", "AvailableWorktime");
            }

            var user = _userManager.GetUserAsync(User).Result;

            for (int index = 0; index < model.Start.Count; index++)
            {
                if (model.Start[index].CompareTo(model.Finish[index]) > 0)
                {
                    return RedirectToAction("Create", "AvailableWorktime");
                }
            }

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
                if (availableWorktime.SchoolHoursWorked < 0)
                {
                    availableWorktime.SchoolHoursWorked = 0;
                }
                if (availableWorktime.Start > availableWorktime.Finish)
                {
                    return RedirectToAction("Create", "AvailableWorktime");
                }
                _context.Add(availableWorktime);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Standard", "AvailableWorktime");
        }

        [HttpGet("AvailableWorktime/Edit/{UserId}/{WorkDate}")]
        public async Task<IActionResult> Edit(int? UserId, string WorkDate)
        {

            if (UserId == null || WorkDate == null)
            {
                return NotFound();
            }

            var decoded = HttpUtility.UrlDecode(WorkDate);
            DateTime date = DateTime.Parse(decoded);

            if (date.Subtract(DateTime.Today.AddDays(7)).Days < 0)
            {
                return RedirectToAction("Standard", "AvailableWorktime");
            }

            var user = _userManager.GetUserAsync(User).Result;
            ViewBag.UserAge = (int)((DateTime.Now - user.DateOfBirth).TotalDays / 365);

            
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

            if (availableWorktime.WorkDate.Subtract(DateTime.Today.AddDays(7)).Days < 0)
            {
                return RedirectToAction("Edit", "AvailableWorktime", new {UserId= userId , WorkDate = availableWorktime.WorkDate.ToString()});
            }

            if (availableWorktime.Start.CompareTo(availableWorktime.Finish) > 0)
            {
                return RedirectToAction("Edit", "AvailableWorktime", new { UserId = userId, WorkDate = availableWorktime.WorkDate.ToString() });
            }

            if (availableWorktime.SchoolHoursWorked < 0)
            {
                availableWorktime.SchoolHoursWorked = 0;
            }
            AvailableWorktime toBeUpdated = _context.AvailableWorktime.Where(a => a.UserId == userId && a.WorkDate == availableWorktime.WorkDate).FirstOrDefault();
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
            return RedirectToAction("Standard", "AvailableWorktime");
        }

        [HttpGet("AvailableWorktime/Delete/{UserId}/{WorkDate}")]
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

        [HttpGet("AvailableWorktime/DeleteConfirmed/{UserId}/{WorkDate}")]
        public async Task<IActionResult> DeleteConfirmed(int UserId, string WorkDate)
        {
            DateTime workDate = DateTime.Parse(HttpUtility.UrlDecode(WorkDate));
            if (workDate.Subtract(DateTime.Today.AddDays(7)).Days < 0)
            {
                return RedirectToAction(nameof(Index));
            }
            var availableWorktime = await _context.AvailableWorktime.Where(at => at.UserId == UserId && at.WorkDate == workDate).FirstOrDefaultAsync();
            _context.AvailableWorktime.Remove(availableWorktime);
            await _context.SaveChangesAsync();
            return RedirectToAction("Standard", "AvailableWorktime");
        }

        private bool AvailableWorktimeExists(DateTime id)
        {
            return _context.AvailableWorktime.Any(e => e.WorkDate == id);
        }
    }
}
