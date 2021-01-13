using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Bumbo.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Bumbo.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Bumbo.Web.Models;
using System;

namespace Bumbo.Web.Controllers
{
    [Authorize]
    public class ScheduleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ScheduleController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Plan()
        {
            var user = _userManager.GetUserAsync(User).Result;

            var users = _context.Users.ToList().Where(u => u.BranchId == user.BranchId);

            var model = new PlanViewModel
            {
                Users = users
            };

            return View(model);
        }

        public async Task<IActionResult> CreateAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            var model = new PlanCreateViewModel
            {
                User = user
            };

            return View(model);
        }
    }
}
