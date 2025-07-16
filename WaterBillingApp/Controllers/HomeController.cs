using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WaterBillingApp.Data.Entities;

/// <summary>
/// Controller responsible for handling the application's home page and user redirection based on roles.
/// </summary>
public class HomeController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="HomeController"/> class.
    /// </summary>
    /// <param name="userManager">The UserManager instance to manage application users.</param>
    public HomeController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    /// <summary>
    /// Serves the home page and redirects authenticated users to their respective dashboards based on roles.
    /// If the user is not authenticated or email is not confirmed, the default home view is returned.
    /// </summary>
    /// <returns>
    /// A view result representing the home page or a redirect to the appropriate role-based dashboard.
    /// </returns>
    public async Task<IActionResult> Index()
    {
        // Check if the user is authenticated
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            // Retrieve the current logged-in user object
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // If email is not confirmed, show default view (e.g. confirmation alert)
                if (!user.EmailConfirmed)
                {
                    return View();
                }

                // Redirect admins to Admin dashboard
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                // Redirect customers to CustomerArea dashboard
                if (await _userManager.IsInRoleAsync(user, "Customer"))
                {
                    return RedirectToAction("Index", "CustomerArea");
                }
                // Redirect employees to Employee dashboard
                if (await _userManager.IsInRoleAsync(user, "Employee"))
                {
                    return RedirectToAction("Index", "Employee");
                }
            }
        }

        // If user is not authenticated or no roles matched, show default home page
        return View();
    }

    /// <summary>
    /// Displays the admin panel view, passing the current logged-in user to the view via ViewBag.
    /// </summary>
    /// <returns>A view result representing the admin panel page.</returns>
    public async Task<IActionResult> AdminPanel()
    {
        // Get current logged-in user to pass to the view
        var currentUser = await _userManager.GetUserAsync(User);

        // Pass user object to the view using ViewBag
        ViewBag.CurrentUser = currentUser;

        // Return the AdminPanel view
        return View();
    }

}
