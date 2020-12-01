using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Bumbo.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
namespace Bumbo.Web.Areas.Identity.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;

        public ChangePasswordModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<ChangePasswordModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Huidig wachtwoord")]
            public string OldPassword { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "Je wachtwoord moet een minimale lengte hebben van {2} en maximaal {1}.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Nieuw wachtwoord")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Herhaal nieuw wachtwoord")]
            [Compare("NewPassword", ErrorMessage = "De nieuwe wachtwoorden kwamen niet overeen!")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Kon gebruiker met ID '{_userManager.GetUserId(User)}' niet laden.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToPage("./SetPassword");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Kon gebruiker met ID '{_userManager.GetUserId(User)}' niet laden.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation($"Gebruiker ({user.UserName}) heeft zijn wachtwoord verwijderd");
            StatusMessage = "Je wachtwoord is gewijzigd.";

            return RedirectToPage();
        }
    }
}
