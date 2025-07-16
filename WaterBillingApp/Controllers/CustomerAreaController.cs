using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Helpers;
using WaterBillingApp.Models;

/// <summary>
/// Controller responsible for managing the customer area, including dashboards, meter requests,
/// invoices, and meter listings.
/// </summary>
[Authorize(Roles = "Customer")]
public class CustomerAreaController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMeterRepository _meterRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly INotificationRepository _notificationRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerAreaController"/> class.
    /// </summary>
    public CustomerAreaController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IMeterRepository meterRepository,
        ICustomerRepository customerRepository,
        IInvoiceRepository invoiceRepository,
        INotificationRepository notificationRepository)
    {
        _context = context;
        _userManager = userManager;
        _meterRepository = meterRepository;
        _customerRepository = customerRepository;
        _invoiceRepository = invoiceRepository;
        _notificationRepository = notificationRepository;
    }

    /// <summary>
    /// Displays the customer's dashboard, including pending invoices and unread notifications.
    /// Marks all notifications as read upon loading.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        // Get the ID of the logged-in user
        var userId = _userManager.GetUserId(User);

        // Get the customer entity associated with the user
        var customer = await _customerRepository.GetByUserIdAsync(userId);

        // Fetch any pending invoice for the customer
        var pendingInvoice = await _invoiceRepository.GetPendingInvoiceForCustomerAsync(customer.Id);

        // Get all unread notifications for the customer
        var notifications = await _notificationRepository.GetUnreadNotificationsAsync(customer.Id);

        // Build the dashboard view model
        var model = new CustomerDashboardViewModel
        {
            CustomerName = customer.FullName,
            HasPendingInvoice = pendingInvoice != null,
            PendingInvoiceId = pendingInvoice?.Id,
            Notifications = notifications
        };

        // Mark all notifications as read and save changes
        await _notificationRepository.MarkAllAsReadAsync(customer.Id);
        await _notificationRepository.SaveChangesAsync();

        // Return the dashboard view with the model
        return View(model);
    }

    /// <summary>
    /// Handles a meter request from the customer. A new meter is created in a pending state
    /// and linked to the current customer.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> RequestMeter()
    {
        // Get the logged-in user
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound();

        // Find the customer linked to the user
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);
        if (customer == null)
            return NotFound();

        // Create a new meter with default values (inactive, with unique serial number)
        var meter = new Meter
        {
            SerialNumber = "REQ-" + Guid.NewGuid().ToString().Substring(0, 8),
            InstallationDate = DateTime.Now,
            IsActive = false,
            CustomerId = customer.Id
        };

        // Add and save the new meter
        _context.Meters.Add(meter);
        await _context.SaveChangesAsync();

        // Show success message and redirect to dashboard
        TempData["StatusMessage"] = "Meter request submitted. Awaiting approval.";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Displays the list of invoices issued to the logged-in customer.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Invoices()
    {
        // Get the ID of the logged-in user
        var userId = _userManager.GetUserId(User);

        // Get the customer by user ID
        var customer = await _customerRepository.GetByUserIdAsync(userId);
        if (customer == null)
            return NotFound();

        // Get all invoices issued to this customer
        var invoices = await _invoiceRepository.GetInvoicesByCustomerIdAsync(customer.Id);

        // Build the view model
        var model = new CustomerInvoicesViewModel
        {
            CustomerName = customer.FullName,
            Invoices = invoices.ToList()
        };

        // Return the invoices view
        return View(model);
    }

    /// <summary>
    /// Displays all meters associated with the currently logged-in customer.
    /// </summary>
    public async Task<IActionResult> Meters()
    {
        // Get the ID of the logged-in user
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        // Get the customer entity
        var customer = await _customerRepository.GetByUserIdAsync(userId);
        if (customer == null)
            return NotFound("Customer not found");

        // Get all meters linked to this customer
        var meters = await _meterRepository.GetMetersByCustomerAsync(customer.Id);

        // Return the meters view
        return View(meters);
    }
}
