using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Bumbo.Data.Models;
using Bumbo.Data.Repository;
using Bumbo.Web.Models;
using System;
using Bumbo.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Bumbo.Web.Controllers
{
    public class PrognosesController : Controller
    {
        private IPrognosesRepository repo = new PrognosesRepository();
        private readonly ApplicationDbContext context;

        public PrognosesController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            var jan1 = new DateTime(DateTime.Today.Year, 1, 1);
            var startOfFirstWeek = jan1.AddDays(1 - (int)(jan1.DayOfWeek));
            var weeks =
                Enumerable
                    .Range(0, 54)
                    .Select(i => new
                    {
                        weekStart = startOfFirstWeek.AddDays(i * 7)
                    })
                    .TakeWhile(x => x.weekStart.Year <= jan1.Year)
                    .Select(x => new
                    {
                        x.weekStart,
                        weekFinish = x.weekStart.AddDays(4)
                    })
                    .SkipWhile(x => x.weekFinish < jan1.AddDays(1))
                    .Select((x, i) => new
                    {
                        Van = x.weekStart.ToShortDateString(),
                        Tot = x.weekFinish.ToShortDateString(),
                        //WeekNummer = i + 1
                    });

            ViewBag.Weeks = weeks;

            return View();
        }

        [HttpPost]
        public IActionResult IndexPrognoses(string week)
        {
            week = Regex.Replace(week, "[^0-9]+", string.Empty);
            int weekNr = Int32.Parse(week);
            DateTime start = FirstDateOfWeek(2020, weekNr, CultureInfo.CurrentCulture);
            DateTime end = start.AddDays(6);

            var jan1 = new DateTime(DateTime.Today.Year, 1, 1);
            var startOfFirstWeek = jan1.AddDays(1 - (int)(jan1.DayOfWeek));
            var weeks =
                Enumerable
                    .Range(0, 54)
                    .Select(i => new
                    {
                        weekStart = startOfFirstWeek.AddDays(i * 7)
                    })
                    .TakeWhile(x => x.weekStart.Year <= jan1.Year)
                    .Select(x => new
                    {
                        x.weekStart,
                        weekFinish = x.weekStart.AddDays(4)
                    })
                    .SkipWhile(x => x.weekFinish < jan1.AddDays(1))
                    .Select((x, i) => new
                    {
                        Van = x.weekStart.ToShortDateString(),
                        Tot = x.weekFinish.ToShortDateString(),
                        //WeekNummer = i + 1
                    });

            ViewBag.Weeks = weeks;
            ViewBag.Prognoses = repo.GetAll(start, end);
            ViewBag.Start = start;
            ViewBag.End = end;

            return View("Index");
        }

        public static DateTime FirstDateOfWeek(int year, int weekOfYear, System.Globalization.CultureInfo ci)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = (int)ci.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;
            DateTime firstWeekDay = jan1.AddDays(daysOffset);
            int firstWeek = ci.Calendar.GetWeekOfYear(jan1, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);
            if ((firstWeek <= 1 || firstWeek >= 52) && daysOffset >= -3)
            {
                weekOfYear -= 1;
            }
            return firstWeekDay.AddDays(weekOfYear * 7);
        }

        public IActionResult Details(DateTime date, int branchId)
        {
            var model = repo.Get(date.Date, branchId);
            return View(model);
        }

        public IActionResult Create(DateTime start)
        {
            var prognoses = new List<PrognoseViewModel>();

            for (int i = 0; i < 7; i++)
            {
                prognoses.Add(
                    new PrognoseViewModel()
                    {
                        Date = start.AddDays(i)
                    }
                );
            }

            PrognosesViewModel viewModel = new PrognosesViewModel()
            {
                Prognoses = prognoses
            };

            ViewBag.Branches = context.Branch.ToList();

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Store(PrognosesViewModel model)
        {
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(DateTime date, int branchId)
        {
            if (TempData["Error"] != null) ViewBag.Error = TempData["Error"].ToString();

            var model = repo.Get(date.Date, branchId);

            return View("Edit", model);
        }

        [HttpPost]
        public IActionResult Update(DateTime Date, Prognoses updatedProg, int BranchId)
        {
            if (Date.Date != updatedProg.Date) return NotFound();

            if (updatedProg.AmountOfCustomers == 0)
            {
                TempData["Error"] = "Veld mag niet leeg zijn";
                return RedirectToAction("Edit", new { Date, BranchId });
            }


            repo.Update(updatedProg);
            return RedirectToAction("Edit", new { Date, BranchId });
        }

        [HttpPost]
        public IActionResult Delete(DateTime date, int branchId)
        {
            if (repo.Get(date.Date, branchId) != null)
            {
                repo.Delete(date.Date, branchId);
            }
            return RedirectToAction("Index");
        }
    }
}
