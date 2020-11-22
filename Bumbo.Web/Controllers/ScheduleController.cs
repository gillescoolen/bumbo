using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Bumbo.Data;

//todo: implement data binding
namespace Bumbo.Web.Controllers
{
  [Authorize]
  public class ScheduleController : Controller
  {
    public ScheduleController()
    {
    }

    public IActionResult Index()
    {
      return View();
    }
  }
}
