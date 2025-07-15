using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Helpers;
using WaterBillingApp.Models;

namespace WaterBillingApp.Controllers
{
    public class MeterRequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private INotificationRepository _notificationRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMeterRepository _meterRepository;


        public MeterRequestController(ApplicationDbContext context, INotificationRepository notificationRepository, ICustomerRepository customerRepository, IMeterRepository meterRepository)
        {
            _notificationRepository = notificationRepository;
            _customerRepository = customerRepository;
            _meterRepository = meterRepository;
            _context = context;
        }

        [HttpGet]
        public IActionResult CreateRequest()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> CreateRequest(MeterRequestViewModel model)
        {
            if (ModelState.IsValid)
            return View(model);
            

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

            
            _context.MeterRequests.Add(meterRequest);
            await _context.SaveChangesAsync();

          
            var notification = new Notification
            {
                Message = $"New meter request submitted by {meterRequest.RequesterName}.",
                CreatedAt = DateTime.Now,
                IsRead = false,
                ForEmployee = true
            };

            await _notificationRepository.AddNotificationAsync(notification);
            await _notificationRepository.SaveChangesAsync();

            TempData["StatusMessage"] = "Meter request submitted successfully. We will contact you soon!";
            return RedirectToAction("CreateRequest");
        }


        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Details(int id)
        {
            var request = await _context.MeterRequests.FindAsync(id);
            if (request == null) return NotFound();

            return View(request);
        }

        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> ApproveRequest(int id)
        {
            var request = await _context.MeterRequests.FindAsync(id);
            if (request == null) return NotFound();

            var customer = new Customer
            {
                FullName = request.RequesterName,
                Email = request.RequesterEmail,
                NIF = request.NIF,
                Address = request.Address,
                Phone = request.Phone,
                IsActive = true,
                ApplicationUserId = null
            };

            await _customerRepository.AddAsync(customer);
            
            var meter = new Meter
            {
                CustomerId = customer.Id,
                SerialNumber = GenerateSerialNumber(),
                IsActive = true,
                Status = MeterStatus.Approved
            };

            await _meterRepository.AddAsync(meter);

            request.Status = MeterRequestStatus.Approved;
            await _context.SaveChangesAsync();

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
        private string GenerateSerialNumber()
        {
            return "MTR-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }

        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> RejectRequest(int id)
        {
            var request = await _context.MeterRequests.FindAsync(id);
            if (request == null) return NotFound();

            request.Status = MeterRequestStatus.Rejected;
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "Meter request rejected.";
            return RedirectToAction("Index", "Employee");
        }




    }
}
