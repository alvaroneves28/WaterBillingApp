using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WaterBillingApp.Data;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Helpers;
using WaterBillingApp.Models;
using WaterBillingApp.Repositories;

[Authorize(Roles = "Customer")]
public class CustomerAreaController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMeterRepository _meterRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly INotificationRepository _notificationRepository;

    public CustomerAreaController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMeterRepository meterRepository, ICustomerRepository customerRepository, IInvoiceRepository invoiceRepository, INotificationRepository notificationRepository)
    {
        _context = context;
        _userManager = userManager;
        _meterRepository = meterRepository;
        _customerRepository = customerRepository;
        _invoiceRepository = invoiceRepository;
        _notificationRepository = notificationRepository;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var customer = await _customerRepository.GetByUserIdAsync(userId);

        var pendingInvoice = await _invoiceRepository.GetPendingInvoiceForCustomerAsync(customer.Id);

        var notifications = await _notificationRepository.GetUnreadNotificationsAsync(customer.Id);

        var model = new CustomerDashboardViewModel
        {
            CustomerName = customer.FullName,
            HasPendingInvoice = pendingInvoice != null,
            PendingInvoiceId = pendingInvoice?.Id,
            Notifications = notifications
        };

        
        await _notificationRepository.MarkAllAsReadAsync(customer.Id);
        await _notificationRepository.SaveAsync();

        return View(model);
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

    [HttpGet]
    public async Task<IActionResult> Invoices()
    {
        var userId = _userManager.GetUserId(User);
        var customer = await _customerRepository.GetByUserIdAsync(userId);

        if (customer == null)
            return NotFound();

        var invoices = await _invoiceRepository.GetInvoicesByCustomerIdAsync(customer.Id);

        var model = new CustomerInvoicesViewModel
        {
            CustomerName = customer.FullName,
            Invoices = invoices.ToList()
        };

        return View(model);
    }

    public async Task<IActionResult> Meters()
    {
        
        var userId = _userManager.GetUserId(User);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();


        var customer = await _customerRepository.GetByUserIdAsync(userId);
        if (customer == null)
            return NotFound("Customer not found");

        var meters = await _meterRepository.GetMetersByCustomerAsync(customer.Id);


        return View(meters);
    }





}
