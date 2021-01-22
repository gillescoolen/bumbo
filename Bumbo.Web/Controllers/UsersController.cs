using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bumbo.Data.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Bumbo.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Bumbo.Data;
using Microsoft.CodeAnalysis.CSharp;

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
        public async Task<IActionResult> Create(CreateViewModel createViewModel)
        {
            if (ModelState.IsValid)
            {
                User dataUser = new User()
                {
                    FirstName = createViewModel.FirstName,
                    LastName = createViewModel.LastName,
                    DateOfBirth = createViewModel.DateOfBirth,
                    StreetName = createViewModel.StreetName,
                    HouseNumber = createViewModel.HouseNumber,
                    HouseNumberLetter = createViewModel.HouseNumberLetter,
                    PostalCode = createViewModel.PostalCode,
                    PhoneNumber = createViewModel.PhoneNumber,
                    IBAN = createViewModel.IBAN,
                    BranchId = createViewModel.BranchId,
                    DateOfEmployment = createViewModel.DateOfEmployment,
                    Bid = createViewModel.Bid,
                    Email = createViewModel.Email,
                    UserName = createViewModel.Email
                };

                if (!_context.Branch.Any(branch => branch.Id == dataUser.BranchId))
                {
                    ViewBag.Branches = _context.Branch.ToList();
                    return View(createViewModel);
                }

                var result = await _userManager.CreateAsync(dataUser, createViewModel.Password);

                if (result.Succeeded)
                {
                    if (createViewModel.Role == UserViewModel.Roles.Manager) await _userManager.AddToRoleAsync(dataUser, "Manager");

                    return RedirectToAction(nameof(Index));
                }
            }

            ViewBag.Branches = _context.Branch.ToList();
            return View(createViewModel);
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

                if (!_context.Branch.Any(branch => branch.Id == dataUser.BranchId))
                {
                    ViewBag.Branches = _context.Branch.ToList();
                    return View(userViewModel);
                }
                
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
            
            // delete everything from the user
            var userAtw = _context.ActualTimeWorked.Where(worked => worked.UserId == user.Id);
            _context.ActualTimeWorked.RemoveRange(userAtw);
            
            var userAwt = _context.AvailableWorktime.Where(worked => worked.UserId == user.Id);
            _context.AvailableWorktime.RemoveRange(userAwt);
            
            var userFr = _context.FurloughRequest.Where(worked => worked.UserId == user.Id);
            _context.FurloughRequest.RemoveRange(userFr);
            
            var userPwt = _context.PlannedWorktime.Where(worked => worked.UserId == user.Id);
            _context.PlannedWorktime.RemoveRange(userPwt);

            await _context.SaveChangesAsync();
            
            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Index));
        }
    }
}
