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
using Microsoft.EntityFrameworkCore;

namespace Bumbo.Web.Controllers
{
    public class PrognosesController : Controller
    {
        private readonly IPrognosesRepository _repository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public PrognosesController(ApplicationDbContext context, UserManager<User> userManager, IPrognosesRepository repository)
        {
            _context = context;
            _userManager = userManager;
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
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
                        weekFinish = x.weekStart.AddDays(6)
                    })
                    .SkipWhile(x => x.weekFinish < jan1.AddDays(1))
                    .Select((x, i) => new
                    {
                        Van = x.weekStart.ToShortDateString(),
                        Tot = x.weekFinish.ToShortDateString(),
                        //WeekNummer = i + 1
                    });


            ViewBag.HasPrognoses = HasPrognoses(weeks, user.BranchId);

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
                        weekFinish = x.weekStart.AddDays(6)
                    })
                    .SkipWhile(x => x.weekFinish < jan1.AddDays(1))
                    .Select((x, i) => new
                    {
                        Van = x.weekStart.ToShortDateString(),
                        Tot = x.weekFinish.ToShortDateString(),
                        //WeekNummer = i + 1
                    });

            var user = await _userManager.GetUserAsync(User);
            var branchId = user.BranchId;

            ViewBag.BranchId = branchId;

            ViewBag.Weeks = weeks;
            ViewBag.Prognoses = _repository.GetAll(start, end, branchId);
            ViewBag.Start = start;
            ViewBag.End = end;

            ViewBag.HasPrognoses = HasPrognoses(weeks, user.BranchId);

            return View("Index");
        }

        private static DateTime FirstDateOfWeek(int year, int weekOfYear, CultureInfo ci)
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
                        LastWeekVisitors = _repository.Get(prognoseDate.AddDays(-7), branchId)?.AmountOfCustomers ?? 0,
                        LastYearVisitors = _repository.Get(prognoseDate.AddYears(-1), branchId)?.AmountOfCustomers ?? 0
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
        public async Task<IActionResult> Store(PrognosesViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            foreach (var prognoseViewModel in model.Prognoses)
            {
                if (prognoseViewModel.Date != null && prognoseViewModel.BranchId != 0)
                {
                    if (prognoseViewModel.BranchId != user.BranchId) return Unauthorized();

                    Prognoses prognose = new Prognoses
                    {
                        Date = prognoseViewModel.Date,
                        AmountOfCustomers = prognoseViewModel.AmountOfCustomers,
                        AmountOfFreight = prognoseViewModel.AmountOfFreight,
                        BranchId = prognoseViewModel.BranchId,
                        WeatherDescription = prognoseViewModel.WeatherDescription,
                        Branch = prognoseViewModel.Branch
                    };

                    var exists = await _context.Prognoses
                        .Where(p => p.Date == prognose.Date)
                        .AnyAsync();

                    if (exists)
                        _repository.Update(prognose);

                    if (!_repository.Create(prognose)) return View("Create", model);
                }
                else
                {
                    return View("Create", model);
                }
            }


            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Edit(DateTime start, int branchId)
        {
            var prognoses = new List<PrognoseViewModel>();

            for (int i = 0; i < 7; i++)
            {
                var prognoseDate = start.AddDays(i);
                var prog = _repository.Get(prognoseDate, branchId);
                if (prog != null)
                {
                    prognoses.Add(
                    new PrognoseViewModel()
                    {
                        Date = prog.Date,
                        BranchId = branchId,
                        AmountOfCustomers = prog.AmountOfCustomers,
                        AmountOfFreight = prog.AmountOfFreight,
                        WeatherDescription = prog.WeatherDescription,
                        LastWeekVisitors = _repository.Get(prognoseDate.AddDays(-7), branchId)?.AmountOfCustomers ?? 0,
                        LastYearVisitors = _repository.Get(prognoseDate.AddYears(-1), branchId)?.AmountOfCustomers ?? 0

                    });

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
            }

            PrognosesViewModel viewModel = new PrognosesViewModel()
            {
                Prognoses = prognoses
            };
            ViewBag.Branches = _context.Branch.ToList();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(PrognosesViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            foreach (var prognoseViewModel in model.Prognoses)
            {
                if (prognoseViewModel.Date != null && prognoseViewModel.BranchId != 0)
                {
                    if (prognoseViewModel.BranchId != user.BranchId) return Unauthorized();

                    Prognoses prognose = new Prognoses
                    {
                        Date = prognoseViewModel.Date,
                        AmountOfCustomers = prognoseViewModel.AmountOfCustomers,
                        AmountOfFreight = prognoseViewModel.AmountOfFreight,
                        BranchId = prognoseViewModel.BranchId,
                        WeatherDescription = prognoseViewModel.WeatherDescription,
                        Branch = prognoseViewModel.Branch
                    };

                    if (!_repository.Update(prognose)) return View("Edit", model);
                }
                else
                {
                    return View("Edit", model);
                }
            }


            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(DateTime start, int branchId)
        {
            for (int i = 0; i < 7; i++)
            {
                var prognoseDate = start.AddDays(i);
                if (_repository.Get(prognoseDate, branchId) != null)
                    _repository.Delete(prognoseDate, branchId);
            }

            return RedirectToAction("Index");
        }

        private List<bool> HasPrognoses(IEnumerable<object> weeks, int branchId)
        {
            List<bool> b = new List<bool>();
            var weekNr = 1;

            for (int i = 0; i < weeks.Count(); i++)
            {
                DateTime start = FirstDateOfWeek(DateTime.Now.Year, weekNr, CultureInfo.CurrentCulture);
                DateTime end = start.AddDays(6);

                if (_repository.GetAll(start, end, branchId).Count() > 0)
                    b.Add(true);
                else
                    b.Add(false);

                weekNr++;
            }

            return b;

        }
    }
}
