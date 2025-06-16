using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WaterBillingApp.Data.Entities;

public class HomeController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                if (!user.EmailConfirmed)
                {
                    // Mostra página de confirmação/alerta ou deixa o utilizador na Home
                    return View();
                }

                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (await _userManager.IsInRoleAsync(user, "Customer"))
                {
                    return RedirectToAction("Index", "CustomerArea");
                }
                if (await _userManager.IsInRoleAsync(user, "Employee"))
                {
                    return RedirectToAction("Index", "Employee");
                }
            }
        }

        return View();
    }

    public async Task<IActionResult> AdminPanel()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        ViewBag.CurrentUser = currentUser;
        return View();
    }

}
