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
                        //WeekStart = x.weekStart,
                        //WeekEnd = x.weekFinish,
                        Week = i + 1
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
            DateTime end = start.AddDays(7);

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
                        //WeekStart = x.weekStart,
                        //WeekEnd = x.weekFinish,
                        Week = i + 1
                    });

            ViewBag.Weeks = weeks;

            ViewBag.Prognoses = repo.GetAll(start, end);


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




        public IActionResult Show(DateTime date)
        {
            ViewBag.Prognoses = repo.Get(date.Date);
            return View("Details");
        }

        public IActionResult Create()
        {
            var model = new Prognoses();
            ViewBag.Branches = context.Branch.ToList();
            return View("Create", model);
        }

        [HttpPost]
        public IActionResult Store(Prognoses prog)
        {
            if (prog.AmountOfCustomers == 0)
            {
                TempData["Error"] = "Veld mag niet leeg zijn";
                return RedirectToAction("Create");
            }

            var model = repo.Create(prog);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(DateTime date)
        {
            if (TempData["Error"] != null) ViewBag.Error = TempData["Error"].ToString();

            var model = repo.Get(date.Date);

            return View("Edit", model);
        }

        [HttpPost]
        public IActionResult Update(DateTime date, Prognoses updatedProg)
        {
            if (date.Date != updatedProg.Date) return NotFound();

            if (updatedProg.AmountOfCustomers == 0)
            {
                TempData["Error"] = "Veld mag niet leeg zijn";
                return RedirectToAction("Edit", new { date });
            }


            repo.Update(updatedProg);
            return RedirectToAction("Edit", new { date });
        }

        [HttpPost]
        public IActionResult Delete(DateTime date)
        {
            if (repo.Get(date.Date) != null)
            {
                repo.Delete(date.Date);
            }
            return RedirectToAction("Index");
        }
    }
}
