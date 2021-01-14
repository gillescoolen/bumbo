using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Bumbo.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Bumbo.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using Bumbo.Web.Models;

namespace Bumbo.Web.Controllers
{
    [Route("api/schedule")]
    [ApiController]
    [Authorize]
    public class ScheduleApiController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ScheduleApiController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<List<ApiViewModel>>> GetPlannedWorkTime(DateTime start, DateTime end)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var plannedWorktimes = await _context.PlannedWorktime
                .Where(p => p.WorkDate >= start)
                .Where(p => p.WorkDate <= end )
                .Where(p => p.UserId == user.Id)
                .ToListAsync();

            var times = new List<ApiViewModel>();

            foreach (var time in plannedWorktimes)
            {
                times.Add(new ApiViewModel
                {
                    Title = $"Werken - {time.Section}",
                    Start = time.WorkDate.AddHours(time.Start.TotalHours),
                    End = time.WorkDate.AddHours(time.Finish.TotalHours)
                });
            }

            return times;
        }
    }
}
