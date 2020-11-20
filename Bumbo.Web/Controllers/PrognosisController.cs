using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Bumbo.Data.Models;
using Bumbo.Data.Repository;
using Bumbo.Web.Models;
using System;

namespace Bumbo.Web.Controllers
{
    public class PrognosisController : Controller
    {
        private IPrognosesRepository _repo = new PrognosesRepository();

        public IActionResult Index()
        {
            if (TempData["Error"] != null) ViewBag.Error = TempData["Error"].ToString();

            ViewBag.Prognoses = _repo.GetAll();
            return View();
        }

        public IActionResult Show(DateTime date)
        {
            if (TempData["Error"] != null) ViewBag.Error = TempData["Error"].ToString();

            ViewBag.Prognoses = _repo.Get(date.Date);
            return View("Detail");
        }

        public IActionResult Create()
        {
            if (TempData["Error"] != null) ViewBag.Error = TempData["Error"].ToString();

            var model = new Prognoses();
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

            var model = _repo.Create(prog);
            return RedirectToAction("Show", new { model.Date });
        }

        public IActionResult Edit(DateTime date)
        {
            if (TempData["Error"] != null) ViewBag.Error = TempData["Error"].ToString();

            var model = _repo.Get(date.Date);

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


            _repo.Update(updatedProg);
            return RedirectToAction("Edit", new { date });
        }

        [HttpPost]
        public IActionResult Delete(DateTime date)
        {
            if (_repo.Get(date.Date) != null)
            {
                _repo.Delete(date.Date);
            }
            return RedirectToAction("Index");
        }
    }
}
