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

namespace Bumbo.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Message> messages = new List<Message>();
            User user = _userManager.GetUserAsync(User).Result;

            // User is an Admin
            if (User.IsInRole("Admin") || true == true) // TODO: Check user role is Admin
            {
                // TODO: Add admin items
            }

            // User is NOT an Admin
            else
            {

            }

            #region Verlofaanvragen
            int requests = _context.FurloughRequest
                .Where(fr => fr.UserId == user.Id)
                .Where(fr => fr.WorkDate > DateTime.Now)
                .Count();

            if (requests > 0)
            {
                int approvedRequests = _context.FurloughRequest
                    .Where(fr => fr.UserId == user.Id)
                    .Where(fr => fr.IsApproved == 1)
                    .Where(fr => fr.WorkDate > DateTime.Now)
                    .Count();

                messages.Add(new Message
                {
                    Priority = Message.Priorities.Low,
                    Type = Message.MessageType.Card,
                    Title = "Geaccepteerde verlofaanvragen",
                    Content = $"<span style='font-size: xx-large; vertical-align: middle;'>{approvedRequests}/{requests}</span> <span style='font-size: large; vertical-align: middle;'>aanvragen zijn geaccepteerd</span>",
                    Location = "/todo/verlofaanvragen"
                });
            }
            #endregion

            #region Prognoses
            // Check if there are prognoses for upcoming dates... (tomorrow - 2 weeks)
            DateTime currentDate = DateTime.Now;
            List<Prognoses> prognoses = _context.Prognoses.Where(p => p.Date > currentDate).Where(p => p.BranchId == user.BranchId).ToList();
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
                    }); ;
                }
            }
            #endregion


            messages.Add(new Message
            {
                Priority = Message.Priorities.Medium,
                Type = Message.MessageType.List,
                Title = "Test!",
                Content = "Dit is een test van tommy",
                RelatedDate = DateTime.Now.AddDays(3)
            }); ;

            return View(new DashboardViewModel() {
                MessagesCards = messages.Where(m => m.Type == Message.MessageType.Card).ToList(),
                MessagesList = messages.Where(m => m.Type == Message.MessageType.List).ToList()
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
