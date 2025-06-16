using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WaterBillingApp.Data;
using WaterBillingApp.Data.Entities;

[Authorize(Roles = "Customer")]
public class CustomerAreaController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CustomerAreaController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound();

        var customer = await _context.Customers
            .Include(c => c.Meters)
            .FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

        if (customer == null)
            return NotFound();

        return View(customer.Meters);
    }

    [HttpGet]
    public async Task<IActionResult> RequestMeter()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound();

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);

        if (customer == null)
            return NotFound();

        var meter = new Meter
        {
            SerialNumber = "REQ-" + Guid.NewGuid().ToString().Substring(0, 8),
            InstallationDate = DateTime.Now,
            IsActive = false,
            CustomerId = customer.Id
        };

        _context.Meters.Add(meter);
        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = "Meter request submitted. Awaiting approval.";
        return RedirectToAction(nameof(Index)); 
    }

}
