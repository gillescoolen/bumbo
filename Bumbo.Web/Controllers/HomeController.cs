using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Bumbo.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Bumbo.Data;
using System.Text.RegularExpressions;
using Bumbo.Data.Models;
using System.Globalization;
using Bumbo.Web.Models.Home;

namespace Bumbo.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            List<Message> messages = GetMessages();

            return View(new DashboardViewModel()
            {
                MessagesCards = messages.Where(m => m.Type == Message.MessageType.Card).ToList(),
                MessagesList = messages.Where(m => m.Type == Message.MessageType.List).ToList()
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        private List<Message> GetMessages()
        {
            List<Message> messages = new List<Message>();
            User user = _userManager.GetUserAsync(User).Result;

            // Fetching from dbContext
            List<Prognoses> prognoses = _context.Prognoses.ToList();

            // User is a manager
            if (User.IsInRole("Manager") || true == true)
            {
                #region Prognoses

                // Check if there are prognoses for upcoming dates... (tomorrow - 2 weeks)
                DateTime currentDate = DateTime.Today;
                List<Prognoses> prognosesCard = prognoses.Where(p => p.Date > currentDate)
                    .Where(p => p.BranchId == user.BranchId).ToList();
                int daysToLookForward = 14;

                for (int i = 0; i < daysToLookForward; i++)
                {
                    DateTime checkDate = currentDate.AddDays(i + 1);

                    if (prognoses.Where(p => p.Date == checkDate).Count() == 0)
                    {
                        messages.Add(new Message
                        {
                            Priority = (i < 7 ? Message.Priorities.High : Message.Priorities.Medium),
                            Type = Message.MessageType.List,
                            Title = "Ontbrekende prognose",
                            Content = $"Er is nog geen prognose aangemaakt voor deze dag!",
                            Location = "/todo/prognoses",
                            RelatedDate = checkDate
                        });
                        ;
                    }
                }

                #endregion

                #region Todays Prognose

                Prognoses todaysPrognose = prognoses.Where(p => p.BranchId == user.BranchId)
                    .Where(p => p.Date == DateTime.Today).FirstOrDefault();

                if (todaysPrognose != null)
                {
                    messages.Add(new Message
                    {
                        Priority = Message.Priorities.Low,
                        Type = Message.MessageType.Card,
                        Title = "Vandaag",
                        Content =
                            $"<span style='font-size: xx-large; vertical-align: middle;'>{todaysPrognose.AmountOfFreight}</span> <span style='font-size: large; vertical-align: middle;'>geplande vracht</span><br/><span style='font-size: xx-large; vertical-align: middle;'>{todaysPrognose.AmountOfCustomers}</span> <span style='font-size: large; vertical-align: middle;'>verwachtte bezoekers</span>"
                    });
                }

                #endregion
            }

            // User is NOT an Admin
            else
            {
            }

            #region Worked Hours

            //This weeks hours
            List<AvailableWorktime> availableWorktimes =
                _context.AvailableWorktime.Where(pwt => pwt.UserId == user.Id).ToList();
            List<ActualTimeWorked> actualTimeWorked =
                _context.ActualTimeWorked.Where(awt => awt.UserId == user.Id).ToList();

            DateTime monday = DateTime.Today.AddDays(
                (int) CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek -
                (int) DateTime.Today.DayOfWeek);

            DateTime[] week =
            {
                monday,
                monday.AddDays(1),
                monday.AddDays(2),
                monday.AddDays(3),
                monday.AddDays(4),
                monday.AddDays(5),
                monday.AddDays(6)
            };

            TimeSpan totalAvailable = new TimeSpan();
            TimeSpan totalActual = new TimeSpan();

            foreach (DateTime day in week)
            {
                AvailableWorktime availableDay =
                    availableWorktimes.Where(available => available.WorkDate == day).FirstOrDefault();
                ActualTimeWorked actualDay = actualTimeWorked.Where(actual => actual.WorkDate == day).FirstOrDefault();

                if (availableDay != null) totalAvailable = totalAvailable.Add(availableDay.Finish - availableDay.Start);
                if (actualDay != null) totalActual = totalActual.Add(actualDay.Finish - actualDay.Start);
            }

            if (totalActual > totalAvailable)
            {
                messages.Add(new Message
                {
                    Priority = Message.Priorities.Low,
                    Type = Message.MessageType.Card,
                    Title = "Deze week",
                    Content =
                        $"<span style='font-size: xx-large; vertical-align: middle;'><span style='color:red;'>{totalActual.TotalHours:00}:{totalActual.Minutes:00}</span>/{totalAvailable.TotalHours:00}:{totalAvailable.Minutes:00}</span> <span style='font-size: large; vertical-align: middle;'>uren gewerkt</span>"
                });
            }
            else
            {
                messages.Add(new Message
                {
                    Priority = Message.Priorities.Low,
                    Type = Message.MessageType.Card,
                    Title = "Deze week",
                    Content =
                        $"<span style='font-size: xx-large; vertical-align: middle;'>{totalActual.TotalHours:00}:{totalActual.Minutes:00}/{totalAvailable.TotalHours:00}:{totalAvailable.Minutes:00}</span> <span style='font-size: large; vertical-align: middle;'>uren gewerkt</span>"
                });
            }

            #endregion

            #region Verlofaanvragen

            List<FurloughRequest> requests = _context.FurloughRequest
                .Where(fr => fr.UserId == user.Id)
                .Where(fr => fr.WorkDate > DateTime.Now)
                .ToList();

            if (requests.Count() > 0)
            {
                int approvedRequests = requests
                    .Where(fr => fr.IsApproved == 1)
                    .Count();

                messages.Add(new Message
                {
                    Priority = Message.Priorities.Low,
                    Type = Message.MessageType.Card,
                    Title = "Verlofaanvragen",
                    Content =
                        $"<span style='font-size: xx-large; vertical-align: middle;'>{approvedRequests}/{requests.Count()}</span> <span style='font-size: large; vertical-align: middle;'>aanvragen zijn geaccepteerd</span>",
                    Location = "/todo/verlofaanvragen"
                });
            }

            #endregion

            return messages;
        }

        public async Task<ActionResult> SubmitHours()
        {
            var user = await _userManager.GetUserAsync(User);

            PlannedWorktime plannedWorktime =
                _context.PlannedWorktime.FirstOrDefault(pwt =>
                    pwt.UserId == user.Id && pwt.WorkDate == DateTime.Today);
            
            if (plannedWorktime != null)
            {
                ActualTimeWorked actualTimeWorked =
                    _context.ActualTimeWorked.FirstOrDefault(atw =>
                        atw.UserId == user.Id && atw.WorkDate == DateTime.Today);

                if (actualTimeWorked == null)
                {
                    return View(new SubmitHoursModel());
                }
            }

            return View("CannotSubmit");
        }

        [HttpPost]
        public async Task<IActionResult> SubmitHours(SubmitHoursModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                ActualTimeWorked actualTimeWorked = new ActualTimeWorked
                {
                    UserId = user.Id,
                    WorkDate = DateTime.Today,
                    Sickness = Convert.ToByte(model.Sick)
                };

                if (model.Sick)
                {
                    PlannedWorktime plannedWorktime =
                        _context.PlannedWorktime.FirstOrDefault(pwt =>
                            pwt.UserId == user.Id && pwt.WorkDate == DateTime.Today);
                    
                    if (plannedWorktime != null)
                    {
                        actualTimeWorked.Start = plannedWorktime.Start;
                        actualTimeWorked.Finish = plannedWorktime.Finish;
                    }
                    else
                    {
                        return View("CannotSubmit");
                    }
                }
                else
                {
                    if (model.Start.CompareTo(model.End) > 0) return View(model);

                    actualTimeWorked.Start = model.Start;
                    actualTimeWorked.Finish = model.End;
                }

                await _context.ActualTimeWorked.AddAsync(actualTimeWorked);
                if (await _context.SaveChangesAsync() > 0)
                {
                    return RedirectToAction("Index");
                }
            }

            return View(model);
        }
    }
}