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

        // GET: AvailableWorktime
        public async Task<IActionResult> Index()
        {
            var UserId = _userManager.GetUserAsync(User).Result.Id;
            var user = _context.Users.Where(a => a.Id == UserId);

            //als rol = niet bevoegd alle users te zien => ziet alleen eigen available worktime
            if (User.IsInRole("Admin"))
            {
                var applicationDbContext = _context.AvailableWorktime.Include(a => a.User);
                return View(await applicationDbContext.ToListAsync());
            }
            else
            {
                var applicationDbContext = _context.AvailableWorktime.Include(a => a.User).Where(a => a.UserId == UserId);
                return View(await applicationDbContext.ToListAsync());
            }
        }

        // GET: AvailableWorktime/Create
        // per week op geven dus: x = SELECT max date where start/finish not null; SELECT x+1,x+2,x+3,x+4,x+5,x+6,x+7
        public IActionResult Create()
        {
            var userBirth = _userManager.GetUserAsync(User).Result.DateOfBirth;
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Bid");
            DateTime maxDate = DateTime.Today;
            ViewBag.UserAge = (int)((maxDate - userBirth).TotalDays / 365);
            List<DateTime> newWeek = new List<DateTime>();
            newWeek.Add(maxDate.AddDays(1));
            newWeek.Add(maxDate.AddDays(2));
            newWeek.Add(maxDate.AddDays(3));
            newWeek.Add(maxDate.AddDays(4));
            newWeek.Add(maxDate.AddDays(5));
            newWeek.Add(maxDate.AddDays(6));
            newWeek.Add(maxDate.AddDays(7));
            ViewBag.newDates = newWeek;
            return View();
        }

        // POST: AvailableWorktime/Create
        //moet alle 7 available worktimes aanmaken
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AvailableWorkTimeViewModel model)
        {
            for (int index = 0; index < model.Start.Count; index++)
            {
                AvailableWorktime availableWorktime = new AvailableWorktime
                {
                    UserId = 1,
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

        // GET: AvailableWorktime/Edit
        //werkt nog niet ivm workdate
        [HttpGet("Edit/{UserId}/{WorkDate}")]
        public async Task<IActionResult> Edit(int? UserId, string? WorkDate)
        {
            if (UserId == null || WorkDate == null)
            {
                return NotFound();
            }

            DateTime date = DateTime.Parse(WorkDate);
            var availableWorktime = await _context.AvailableWorktime.Where(at => at.UserId == UserId && at.WorkDate == date).FirstOrDefaultAsync();
            if (availableWorktime == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Bid", availableWorktime.UserId);
            return View(availableWorktime);
        }

        // POST: AvailableWorktime/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int userId, [Bind("UserId,WorkDate,Start,Finish,SchoolHoursWorked")] AvailableWorktime availableWorktime)
        {
            if (userId != availableWorktime.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(availableWorktime);
                    await _context.SaveChangesAsync();
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Bid", availableWorktime.UserId);
            return View(availableWorktime);
        }

        // GET: AvailableWorktime/Delete
        [HttpGet("Delete/{UserId}/{WorkDate}")]
        public async Task<IActionResult> Delete(int? UserId, string? WorkDate)
        {
            if (UserId == null || WorkDate == null)
            {
                return NotFound();
            }

            DateTime date = DateTime.Parse(WorkDate);
            var availableWorktime = await _context.AvailableWorktime
                .Include(a => a.User)
                .FirstOrDefaultAsync(at => at.UserId == UserId && at.WorkDate == date);

            if (availableWorktime == null)
            {
                return NotFound();
            }

            return View(availableWorktime);
        }

        // POST: AvailableWorktime/DeleteConfirmed
        [HttpGet("DeleteConfirmed/{UserId}/{WorkDate}")]
        public async Task<IActionResult> DeleteConfirmed(int UserId, string WorkDate)
        {
            var availableWorktime = await _context.AvailableWorktime.Where(at => at.UserId == UserId && at.WorkDate == DateTime.Parse(WorkDate)).FirstOrDefaultAsync();
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
