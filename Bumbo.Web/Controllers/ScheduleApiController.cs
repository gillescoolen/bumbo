using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Bumbo.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Bumbo.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Bumbo.Web.Controllers
{
    [Route("api/schedule")]
    [ApiController]
    [Authorize]
    public class ScheduleApiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScheduleApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlannedWorktime>>> GetPlannedWorkTime()
        {
            return await _context.PlannedWorktime.ToListAsync();
        }
    }
}
