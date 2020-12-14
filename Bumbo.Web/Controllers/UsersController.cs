using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bumbo.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Bumbo.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace Bumbo.Web.Controllers
{
    [Authorize(Roles = "Manager")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public UsersController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            ViewBag.CurrentUserId = _userManager.GetUserAsync(User).Result.Id;
            return View(await _context.Users.Include(u => u.Branch).ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dataUser = await _context.Users
                .Include(u => u.Branch)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dataUser == null)
            {
                return NotFound();
            }

            string userRole = _userManager.GetRolesAsync(dataUser).Result.FirstOrDefault();
            UserViewModel.Roles role;
            if (userRole == "Manager") role = UserViewModel.Roles.Manager;
            else role = UserViewModel.Roles.User;

            UserViewModel userViewModel = new Models.UserViewModel
            {
                Id = dataUser.Id,
                Bid = dataUser.Bid,
                BranchId = dataUser.BranchId,
                DateOfBirth = dataUser.DateOfBirth,
                DateOfEmployment = dataUser.DateOfEmployment,
                Email = dataUser.Email,
                FirstName = dataUser.FirstName,
                HouseNumber = dataUser.HouseNumber,
                HouseNumberLetter = dataUser.HouseNumberLetter,
                IBAN = dataUser.IBAN,
                LastName = dataUser.LastName,
                PhoneNumber = dataUser.PhoneNumber,
                PostalCode = dataUser.PostalCode,
                StreetName = dataUser.StreetName,
                Branch = dataUser.Branch,
                Role = role
            };

            ViewBag.Branches = _context.Branch.ToList();
            return View(userViewModel);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewBag.Branches = _context.Branch.ToList();
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                User dataUser = new User();
                dataUser.PasswordHash = new PasswordHasher<User>().HashPassword(dataUser, userViewModel.Password);
                dataUser.UserName = userViewModel.Email;

                var result = await _userManager.CreateAsync(dataUser);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }

                if (userViewModel.Role == UserViewModel.Roles.Manager) await _userManager.AddToRoleAsync(dataUser, "Manager");

            }

            ViewBag.Branches = _context.Branch.ToList();
            return View(userViewModel);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dataUser = await _userManager.FindByIdAsync(id.ToString());
            if (dataUser == null)
            {
                return NotFound();
            }

            string userRole = _userManager.GetRolesAsync(dataUser).Result.FirstOrDefault();
            UserViewModel.Roles role;
            if (userRole == "Manager") role = UserViewModel.Roles.Manager;
            else role = UserViewModel.Roles.User;

            UserViewModel userViewModel = new UserViewModel
            {
                Id = dataUser.Id,
                Bid = dataUser.Bid,
                BranchId = dataUser.BranchId,
                DateOfBirth = dataUser.DateOfBirth,
                DateOfEmployment = dataUser.DateOfEmployment,
                Email = dataUser.Email,
                FirstName = dataUser.FirstName,
                HouseNumber = dataUser.HouseNumber,
                HouseNumberLetter = dataUser.HouseNumberLetter,
                IBAN = dataUser.IBAN,
                LastName = dataUser.LastName,
                PhoneNumber = dataUser.PhoneNumber,
                PostalCode = dataUser.PostalCode,
                StreetName = dataUser.StreetName,
                Branch = dataUser.Branch,
                Role = role
            };

            ViewBag.Branches = _context.Branch.ToList();
            return View(userViewModel);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserViewModel userViewModel)
        {

            if (id != userViewModel.Id)
            {
                return NotFound();
            }

            var dataUser = await _userManager.FindByIdAsync(id.ToString());

            if (ModelState.IsValid)
            {
                dataUser.FirstName = userViewModel.FirstName;
                dataUser.LastName = userViewModel.LastName;
                dataUser.DateOfBirth = userViewModel.DateOfBirth;
                dataUser.StreetName = userViewModel.StreetName;
                dataUser.HouseNumber = userViewModel.HouseNumber;
                dataUser.HouseNumberLetter = userViewModel.HouseNumberLetter;
                dataUser.PostalCode = userViewModel.PostalCode;
                dataUser.Email = userViewModel.Email;
                dataUser.PhoneNumber = userViewModel.PhoneNumber;
                dataUser.IBAN = userViewModel.IBAN;
                dataUser.BranchId = userViewModel.BranchId;
                dataUser.DateOfEmployment = userViewModel.DateOfEmployment;
                dataUser.Bid = userViewModel.Bid;

                UserViewModel.Roles role = userViewModel.Role;

                try
                {
                    var result = await _userManager.UpdateAsync(dataUser);
                    if (!result.Succeeded)
                    {
                        ViewBag.Branches = _context.Branch.ToList();
                        return View(userViewModel);
                    }

                    var roles = _userManager.GetRolesAsync(dataUser).Result;
                    await _userManager.RemoveFromRolesAsync(dataUser, roles.ToArray());

                    if (role == UserViewModel.Roles.Manager)
                    {
                        await _userManager.AddToRoleAsync(dataUser, "Manager");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Branches = _context.Branch.ToList();
            return View(userViewModel);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == _userManager.GetUserAsync(User).Result.Id)
            {
                return NotFound();
            }

            var dataUser = await _userManager.FindByIdAsync(id.ToString());
            dataUser.Branch = _context.Branch.Find(dataUser.BranchId);
            if (dataUser == null)
            {
                return NotFound();
            }

            string userRole = _userManager.GetRolesAsync(dataUser).Result.FirstOrDefault();
            UserViewModel.Roles role;
            if (userRole == "Manager") role = UserViewModel.Roles.Manager;
            else role = UserViewModel.Roles.User;

            UserViewModel userViewModel = new UserViewModel
            {
                Id = dataUser.Id,
                Bid = dataUser.Bid,
                BranchId = dataUser.BranchId,
                DateOfBirth = dataUser.DateOfBirth,
                DateOfEmployment = dataUser.DateOfEmployment,
                Email = dataUser.Email,
                FirstName = dataUser.FirstName,
                HouseNumber = dataUser.HouseNumber,
                HouseNumberLetter = dataUser.HouseNumberLetter,
                IBAN = dataUser.IBAN,
                LastName = dataUser.LastName,
                PhoneNumber = dataUser.PhoneNumber,
                PostalCode = dataUser.PostalCode,
                StreetName = dataUser.StreetName,
                Branch = dataUser.Branch,
                Role = role
            };

            return View(userViewModel);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id == _userManager.GetUserAsync(User).Result.Id)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id.ToString());
            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Index));
        }
    }
}
