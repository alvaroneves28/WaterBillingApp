using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Helpers;
using WaterBillingApp.Models;

namespace WaterBillingApp.Controllers
{
    /// <summary>
    /// Controller for managing meter requests submitted by customers.
    /// Allows anonymous users to submit meter requests,
    /// and employees to view, approve, or reject requests.
    /// </summary>
    public class MeterRequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private INotificationRepository _notificationRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMeterRepository _meterRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeterRequestController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="notificationRepository">Repository for managing notifications.</param>
        /// <param name="customerRepository">Repository for managing customers.</param>
        /// <param name="meterRepository">Repository for managing meters.</param>
        public MeterRequestController(ApplicationDbContext context, INotificationRepository notificationRepository, ICustomerRepository customerRepository, IMeterRepository meterRepository)
        {
            _notificationRepository = notificationRepository;
            _customerRepository = customerRepository;
            _meterRepository = meterRepository;
            _context = context;
        }

        /// <summary>
        /// Displays the meter request submission form.
        /// </summary>
        /// <returns>The meter request creation view.</returns>
        [HttpGet]
        public IActionResult CreateRequest()
        {
            // Displays empty form for meter request submission
            return View();
        }


        /// <summary>
        /// Handles the submission of a new meter request.
        /// </summary>
        /// <param name="model">The data submitted by the requester.</param>
        /// <returns>Redirects back to the creation form on success or redisplays the form on validation failure.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> CreateRequest(MeterRequestViewModel model)
        {
            // FIX: inverted logic – should return view only if model is invalid
            if (!ModelState.IsValid)
                return View(model);

            // Creates new request entity from form input
            var meterRequest = new MeterRequest
            {
                RequesterName = model.Name,
                RequesterEmail = model.Email,
                NIF = model.NIF,
                Address = model.Address,
                Phone = model.Phone,
                RequestDate = DateTime.Now,
                Status = MeterRequestStatus.Pending
            };

            // Saves the request to database
            _context.MeterRequests.Add(meterRequest);
            await _context.SaveChangesAsync();

            // Notifies employees about the new request
            var notification = new Notification
            {
                Message = $"New meter request submitted by {meterRequest.RequesterName}.",
                CreatedAt = DateTime.Now,
                IsRead = false,
                ForEmployee = true
            };

            await _notificationRepository.AddNotificationAsync(notification);
            await _notificationRepository.SaveChangesAsync();

            // Feedback to user
            TempData["StatusMessage"] = "Meter request submitted successfully. We will contact you soon!";
            return RedirectToAction("CreateRequest");
        }


        /// <summary>
        /// Displays the details of a specific meter request.
        /// Only accessible by employees.
        /// </summary>
        /// <param name="id">The ID of the meter request.</param>
        /// <returns>The details view or NotFound if the request does not exist.</returns>
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Details(int id)
        {
            // Retrieves specific request by ID
            var request = await _context.MeterRequests.FindAsync(id);
            if (request == null) return NotFound();

            // Displays request details to employee
            return View(request);
        }


        /// <summary>
        /// Approves a meter request and creates the associated customer and meter.
        /// Notifies the admin to create a user account.
        /// </summary>
        /// <param name="id">The ID of the meter request to approve.</param>
        /// <returns>Redirects to the employee index page.</returns>
        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> ApproveRequest(int id)
        {
            var request = await _context.MeterRequests.FindAsync(id);
            if (request == null) return NotFound();

            // Creates a new customer from the approved request
            var customer = new Customer
            {
                FullName = request.RequesterName,
                Email = request.RequesterEmail,
                NIF = request.NIF,
                Address = request.Address,
                Phone = request.Phone,
                IsActive = true,
                ApplicationUserId = null // Account to be created later
            };

            await _customerRepository.AddAsync(customer);

            // Creates a new meter and associates it with the new customer
            var meter = new Meter
            {
                CustomerId = customer.Id,
                SerialNumber = GenerateSerialNumber(),
                IsActive = true,
                Status = MeterStatus.Approved
            };

            await _meterRepository.AddAsync(meter);

            // Updates original request status
            request.Status = MeterRequestStatus.Approved;
            await _context.SaveChangesAsync();

            // Notifies admin to create the actual user account
            var notification = new Notification
            {
                Message = $"New approved meter request from {customer.FullName}. Please create the user account.",
                CreatedAt = DateTime.Now,
                IsRead = false,
                ForEmployee = false,
                CustomerId = customer.Id
            };

            await _notificationRepository.AddNotificationAsync(notification);

            TempData["StatusMessage"] = "Meter activated and admin notified to create user account.";
            return RedirectToAction("Index", "Employee");
        }


        /// <summary>
        /// Generates a new unique serial number for a meter.
        /// </summary>
        /// <returns>A string representing the serial number.</returns>
        
        private string GenerateSerialNumber()
        {
            return "MTR-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }


        /// <summary>
        /// Rejects a meter request by updating its status.
        /// Only accessible by employees.
        /// </summary>
        /// <param name="id">The ID of the meter request to reject.</param>
        /// <returns>Redirects to the employee index page.</returns>
        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> RejectRequest(int id)
        {
            // Marks the request as rejected
            var request = await _context.MeterRequests.FindAsync(id);
            if (request == null) return NotFound();

            request.Status = MeterRequestStatus.Rejected;
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "Meter request rejected.";
            return RedirectToAction("Index", "Employee");
        }


    }
}
