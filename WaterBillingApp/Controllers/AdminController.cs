using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Helpers;
using WaterBillingApp.Models;
using IEmailSender = WaterBillingApp.Helpers.IEmailSender;



namespace WaterBillingApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly INotificationRepository _notificationRepository;
        private readonly IEmailSender _emailSender;
        private readonly ICustomerRepository _customerRepository;

        public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, INotificationRepository notificationRepository, Helpers.IEmailSender emailSender, ICustomerRepository customerRepository)
        {
            _userManager = userManager;
            _context = context;
            _notificationRepository = notificationRepository;
            _emailSender = emailSender;
            _customerRepository = customerRepository;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var pendingCount = await _notificationRepository
            .CountAsync(n => !n.IsRead && n.Message.Contains("Please create the user account"));

            ViewData["PendingAccountsCount"] = pendingCount;
            return View();
        }


        public async Task<IActionResult> ManageUsers()
        {
            var users = _userManager.Users.ToList();
            var model = new List<UserWithRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                model.Add(new UserWithRolesViewModel
                {
                    User = user,
                    Roles = roles
                });
            }

            return View(model);
        }


        public async Task<IActionResult> EditUser(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                EmailConfirmed = user.EmailConfirmed
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return NotFound();

            user.Email = model.Email;
            user.UserName = model.UserName;
            user.EmailConfirmed = model.EmailConfirmed;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return RedirectToAction(nameof(ManageUsers));

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }


        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("DeleteUserConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var cliente = await _customerRepository.GetByUserIdAsync(user.Id);
            if (cliente != null)
            {
                await _customerRepository.DeleteAsync(cliente.Id);
                
            }


            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
                return RedirectToAction(nameof(ManageUsers));

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(user);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PendingAccounts()
        {
            var notifications = await _notificationRepository
                .GetUnassignedAccountNotificationsAsync();

            return View(notifications);
        }

        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> CreateUserAccount(int meterRequestId)
        {
            var request = await _context.MeterRequests.FindAsync(meterRequestId);
            if (request == null || request.Status != MeterRequestStatus.Approved)
            {
                return NotFound();
            }

            
            var existingUser = await _userManager.FindByEmailAsync(request.RequesterEmail);
            if (existingUser != null)
            {
                TempData["StatusMessage"] = "A user with this email already exists.";
                return RedirectToAction("PendingRequests");
            }

            
            var newUser = new ApplicationUser
            {
                UserName = request.RequesterEmail,
                Email = request.RequesterEmail,
                EmailConfirmed = true // opcional
            };

            var tempPassword = Guid.NewGuid().ToString("N").Substring(0, 8) + "!"; 
            var result = await _userManager.CreateAsync(newUser, tempPassword);

            if (!result.Succeeded)
            {
                TempData["StatusMessage"] = "Failed to create user account: " + string.Join(", ", result.Errors.Select(e => e.Description));
                return RedirectToAction("PendingRequests");
            }

            
            await _userManager.AddToRoleAsync(newUser, "Customer");

          
            var newCustomer = new Customer
            {
                FullName = request.RequesterName,
                Email = request.RequesterEmail,
                NIF = request.NIF,
                Address = request.Address,
                Phone = request.Phone,
                IsActive = true,
                ApplicationUserId = newUser.Id
            };

            await _customerRepository.AddAsync(newCustomer);

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(newUser);
            var resetLink = Url.Action("ResetPassword", "Account", new { token = resetToken, email = newUser.Email }, Request.Scheme);

            await _emailSender.SendEmailAsync(newUser.Email, "Your new account",
                $"Hello {newCustomer.FullName},<br><br>Your account has been created. Click <a href='{resetLink}'>here</a> to set your password.<br><br>Regards,<br>Water Billing App");

            TempData["StatusMessage"] = "User account created and email sent.";
            return RedirectToAction("PendingRequests");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUserFromNotification(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null || notification.IsRead)
                return NotFound();

            var customer = await _customerRepository.GetByIdAsync(notification.CustomerId.Value);
            if (customer == null)
                return NotFound();

            var existingUser = await _userManager.FindByEmailAsync(customer.Email);
            if (existingUser != null)
            {
                TempData["StatusMessage"] = "A user with this email already exists.";
                return RedirectToAction("PendingAccounts");
            }

            var newUser = new ApplicationUser
            {
                UserName = customer.Email,
                Email = customer.Email,
                EmailConfirmed = true,
                FullName = customer.FullName,
                Address = customer.Address,
                Phone = customer.Phone, 
                NIF = customer.NIF,     
                IsActive = true
            };

            var tempPassword = "Aa123456!";
            var result = await _userManager.CreateAsync(newUser, tempPassword);

            if (!result.Succeeded)
            {
                TempData["StatusMessage"] = "Failed to create account: " + string.Join(", ", result.Errors.Select(e => e.Description));
                return RedirectToAction("PendingAccounts");
            }

            await _userManager.AddToRoleAsync(newUser, "Customer");

            customer.ApplicationUserId = newUser.Id;
            await _customerRepository.UpdateAsync(customer);

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(newUser);
            var resetLink = Url.Action("ResetPassword", "Account", new { token = resetToken, email = newUser.Email }, Request.Scheme);

            await _emailSender.SendEmailAsync(newUser.Email, "Account Created",
                $"Hello {customer.FullName},<br><br>Your account has been created. Click <a href='{resetLink}'>here</a> to set your password.<br><br>Thank you.");

            notification.IsRead = true;
            await _notificationRepository.SaveChangesAsync();

            TempData["StatusMessage"] = "Account successfully created and notification marked as read.";
            return RedirectToAction("PendingAccounts");
        }

    }
}
