using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bumbo.Data;
using Bumbo.Data.Models;

namespace Bumbo.Web.Controllers
{
    public class AvailableWorktimeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AvailableWorktimeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AvailableWorktime
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.AvailableWorktime.Include(a => a.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: AvailableWorktime/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Bid");
            return View();
        }

        // POST: AvailableWorktime/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,WorkDate,Start,Finish,SchoolHoursWorked")] AvailableWorktime availableWorktime)
        {
            if (ModelState.IsValid)
            {
                _context.Add(availableWorktime);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Bid", availableWorktime.UserId);
            return View(availableWorktime);
        }

        // GET: AvailableWorktime/Edit
        [HttpGet("Edit/{UserId}/{WorkDate}")]
        public async Task<IActionResult> Edit(int? UserId, string? WorkDate)
        {
            if (UserId == null || WorkDate == null)
            {
                return NotFound();
            }

            DateTime date = DateTime.Parse(WorkDate);
            var availableWorktime = await _context.AvailableWorktime.Where(at=>at.UserId==UserId&&at.WorkDate==date).FirstOrDefaultAsync();
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
        public async Task<IActionResult> Edit([Bind("UserId,WorkDate,Start,Finish,SchoolHoursWorked")] AvailableWorktime availableWorktime)
        {
            //if (workdate != availableWorktime.WorkDate || userid!=availableWorktime.UserId)
            //{
            //    return NotFound();
            //}

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
            if (UserId == null||WorkDate == null)
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
        public async Task<IActionResult> DeleteConfirmed(int UserId,string WorkDate)
        {
            var availableWorktime = await _context.AvailableWorktime.Where(at=>at.UserId==UserId&&at.WorkDate== DateTime.Parse(WorkDate)).FirstOrDefaultAsync();
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
