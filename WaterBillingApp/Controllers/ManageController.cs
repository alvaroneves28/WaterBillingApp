using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Models; // onde vais criar o viewmodel

[Authorize]
public class ManageController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public ManageController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (result.Succeeded)
        {
            await _signInManager.RefreshSignInAsync(user);
            TempData["StatusMessage"] = "Your password has been changed.";
            return RedirectToAction("ChangePassword");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            TempData["ErrorMessage"] = "Could not change your password. Please try again.";
            return View(model);
        }
    }
}
