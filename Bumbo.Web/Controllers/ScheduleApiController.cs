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
        public async Task<ActionResult<IEnumerable<PlannedWorktime>>> GetPlannedWorkTime()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            return await _context.PlannedWorktime
                .Where(p => p.WorkDate >= DateTime.Today.AddDays(-31))
                .Where(p => p.UserId == user.Id)
                .ToListAsync();
        }
    }
}
