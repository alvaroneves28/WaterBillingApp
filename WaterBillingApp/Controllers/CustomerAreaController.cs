using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WaterBillingApp.Data;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Helpers;

[Authorize(Roles = "Customer")]
public class CustomerAreaController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMeterRepository _meterRepository;

    public CustomerAreaController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMeterRepository meterRepository)
    {
        _context = context;
        _userManager = userManager;
        _meterRepository = meterRepository;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);
        if (customer == null) return NotFound();

        var meters = await _meterRepository.GetMetersByCustomerAsync(customer.Id);
        return View(meters);
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
