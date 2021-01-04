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
using Nager.Date;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Bumbo.Web.Controllers
{
    public class PrognosesController : Controller
    {
        private IPrognosesRepository repo = new PrognosesRepository();
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public PrognosesController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var jan1 = new DateTime(DateTime.Now.Year, 1, 1);
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
                        weekFinish = x.weekStart.AddDays(6)
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
        public async Task<IActionResult> IndexPrognoses(string week)
        {
            week = Regex.Replace(week, "[^0-9]+", string.Empty);
            int weekNr = Int32.Parse(week);

            DateTime start = FirstDateOfWeek(DateTime.Now.Year, weekNr, CultureInfo.CurrentCulture);
            DateTime end = start.AddDays(6);

            var jan1 = new DateTime(DateTime.Now.Year, 1, 1);
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
                        weekFinish = x.weekStart.AddDays(6)
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

            var user = await _userManager.GetUserAsync(User);
            ViewBag.BranchId = user.BranchId;


            return View("Index");
        }

        public static DateTime FirstDateOfWeek(int year, int weekOfYear, CultureInfo ci)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = (int)ci.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;
            DateTime firstWeekDay = jan1.AddDays(daysOffset);
            int firstWeek = ci.Calendar.GetWeekOfYear(jan1, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);
            if ((firstWeek <= 1 || firstWeek >= 52) && daysOffset >= -4)
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

        [HttpPost]
        public IActionResult Create(DateTime start, int branchId)
        {
            var prognoses = new List<PrognoseViewModel>();
            
            for (int i = 0; i < 7; i++)
            {
                var prognoseDate = start.AddDays(i);
                prognoses.Add(
                    new PrognoseViewModel()
                    {
                        Date = prognoseDate,
                        BranchId = branchId,
                        LastWeekVisitors = repo.Get(prognoseDate.AddDays(-7), branchId)?.AmountOfCustomers ?? 0,
                        LastYearVisitors = repo.Get(prognoseDate.AddYears(-1), branchId)?.AmountOfCustomers ?? 0
                       
                    }
                );

                //Handle holidays
                if (DateSystem.IsPublicHoliday(prognoses[i].Date, CountryCode.NL))
                {
                    var holidays = DateSystem.GetPublicHoliday(prognoses[i].Date, prognoses[i].Date, CountryCode.NL);
                    foreach (var publicHoliday in holidays)
                    {
                        prognoses[i].Holiday = publicHoliday.LocalName;
                    }
                }
            }

            PrognosesViewModel viewModel = new PrognosesViewModel()
            {
                Prognoses = prognoses
            };

            ViewBag.Branches = _context.Branch.ToList();


            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Store(PrognosesViewModel model)
        {
            for (int i = 0; i < model.Prognoses.Count; i++)
            {
                if (model.Prognoses[i].Date != null && model.Prognoses[i].BranchId != 0)
                {
                    Prognoses p = new Prognoses()
                    {
                        Date = model.Prognoses[i].Date,
                        AmountOfCustomers = model.Prognoses[i].AmountOfCustomers,
                        AmountOfFreight = model.Prognoses[i].AmountOfFreight,
                        BranchId = model.Prognoses[i].BranchId,
                        WeatherDescription = model.Prognoses[i].WeatherDescription,
                        Branch = model.Prognoses[i].Branch
                    };

                    repo.Create(p);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
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
            return RedirectToAction("Index");
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
