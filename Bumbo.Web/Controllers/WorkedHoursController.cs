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
using System.Web;
using Bumbo.Data.Models.PayrollServiceIntegration;

namespace Bumbo.Web.Controllers
{
    public class WorkedHoursController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly CAO _cao;

        public WorkedHoursController(ApplicationDbContext context, UserManager<User> user)
        {
            _context = context;
            _userManager = user;
            _cao = new CAO(_context);
        }

        [HttpGet("WorkedHours/{order}")]
        public async Task<IActionResult> Index(string order)
        {
            User user = _userManager.GetUserAsync(User).Result;
            var userHours = _context.ActualTimeWorked.Include(a => a.User).Where(u => u.UserId == user.Id);

            if (User.IsInRole("Manager"))
            {
                userHours = _context.ActualTimeWorked.Include(a => a.User);
            }

            if (order != "")
            {
                if (order.Equals("Datum"))
                {
                    userHours = userHours.OrderBy(u => u.WorkDate);
                }
                else if (order.Equals("Starttijd"))
                {
                    userHours = userHours.OrderBy(u => u.Start);
                }
                else if (order.Equals("Eindtijd"))
                {
                    userHours = userHours.OrderBy(u => u.Finish);
                }
                else if (order.Equals("Naam"))
                {
                    userHours = userHours.OrderBy(u => u.User.FirstName);
                }
                else if (order.Equals("Ziek"))
                {
                    userHours = userHours.OrderBy(u => u.Sickness);
                }
            }

            ViewBag.user = User;
            return View(await userHours.ToListAsync());
        }

        public IActionResult ViewPerHour(int? userId, string workDate)
        {
            if (userId == null)
            {
                return NotFound();
            }
            ViewBag.user = _context.Users.Where(u => u.Id == userId).FirstOrDefault();
            var decoded = HttpUtility.UrlDecode(workDate);
            DateTime date = DateTime.Parse(decoded);
            ViewBag.date = date.ToString("dd/MM/yyyy");
            ActualTimeWorked actualTimeWorked = _context.ActualTimeWorked.Where(at => at.User.Id == userId && at.WorkDate == date).FirstOrDefault();
            DateTime start = date + actualTimeWorked.Start;
            DateTime finish = date + actualTimeWorked.Finish;
           
            return View(_cao.WorkdaySurcharge(start, finish));
        }

        // GET: WorkedHours/Edit/5
        [HttpGet("WorkedHours/Edit/{UserId}/{WorkDate}")]
        public async Task<IActionResult> Edit(int? UserId, string WorkDate)
        {
            if (UserId == null)
            {
                return NotFound();
            }

            var decoded = HttpUtility.UrlDecode(WorkDate);
            DateTime date = DateTime.Parse(decoded);

            var actualTimeWorked = await _context.ActualTimeWorked
                .Where(at => at.User.Id == UserId && at.WorkDate == date).FirstOrDefaultAsync();
            if (actualTimeWorked == null)
            {
                return NotFound();
            }

            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Bid", actualTimeWorked.UserId);
            return View(actualTimeWorked);
        }

        // POST: WorkedHours/EditConfirmed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditConfirmed(int userId, ActualTimeWorked worktime)
        {
            if (userId != worktime.UserId)
            {
                return NotFound();
            }

            ActualTimeWorked toBeUpdated = _context.ActualTimeWorked
                .Where(a => a.UserId == userId && a.WorkDate == worktime.WorkDate).FirstOrDefault();
            try
            {
                if (ModelState.IsValid)
                {
                    toBeUpdated.Start = worktime.Start;
                    toBeUpdated.Finish = worktime.Finish;
                    toBeUpdated.Sickness = worktime.Sickness;
                    _context.Update(toBeUpdated);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActualTimeWorkedExists(worktime.WorkDate))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction("Standard", "WorkedHours");
        }

        // GET: WorkedHours/Delete/5
        [HttpGet("WorkedHours/Delete/{UserId}/{WorkDate}")]
        public async Task<IActionResult> Delete(int? UserId, string? WorkDate)
        {
            if (UserId == null || WorkDate == null)
            {
                return NotFound();
            }

            var decoded = HttpUtility.UrlDecode(WorkDate);
            DateTime date = DateTime.Parse(decoded);
            var workedTime = await _context.ActualTimeWorked
                .Include(a => a.User)
                .FirstOrDefaultAsync(at => at.UserId == UserId && at.WorkDate == date);

            if (workedTime == null)
            {
                return NotFound();
            }

            return View(workedTime);
        }

        public async Task<IActionResult> DeleteConfirmed(int UserId, string WorkDate)
        {
            DateTime workDate = DateTime.Parse(HttpUtility.UrlDecode(WorkDate));
            var workedTime = await _context.ActualTimeWorked.Where(at => at.UserId == UserId && at.WorkDate == workDate)
                .FirstOrDefaultAsync();
            _context.ActualTimeWorked.Remove(workedTime);
            await _context.SaveChangesAsync();
            return RedirectToAction("Standard", "WorkedHours");
        }

        private bool ActualTimeWorkedExists(DateTime id)
        {
            return _context.ActualTimeWorked.Any(e => e.WorkDate == id);
        }

        public async Task<IActionResult> Payout()
        {
            var workTimes = await _context.ActualTimeWorked.Where(atw => atw.Accepted == true && atw.Payed == false).ToListAsync();

            var payroll = new Payroll();
            foreach (var workTime in workTimes)
            {
                var additions = _cao.WorkdaySurcharge(workTime.WorkDate + workTime.Start, workTime.WorkDate + workTime.Finish);
                var percentages = 0;
                foreach (var keyValuePair in additions)
                {
                    percentages += keyValuePair.Value;
                }

                var dayAddition = percentages / additions.Count;
                
                payroll.Items.Add(new PayrollItem
                {
                    Bid = workTime.User.Bid,
                    Hours = Double.Parse(workTime.Finish.Subtract(workTime.Start).ToString()),
                    Addition = dayAddition
                });
                

                workTime.Payed = true;
                _context.ActualTimeWorked.Update(workTime);
                await _context.SaveChangesAsync();
            }

            await PayrollServiceIntegration.Submit(payroll);
            return RedirectToAction(nameof(Index));
        }
    }
}