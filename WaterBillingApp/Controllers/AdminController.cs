using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Helpers;
using WaterBillingApp.Models;
using IEmailSender = WaterBillingApp.Helpers.IEmailSender;

namespace WaterBillingApp.Controllers
{
    /// <summary>
    /// Controller responsible for administrative functionalities such as user management,
    /// pending account processing, account creation, and notification handling.
    /// </summary>
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

        /// <summary>
        /// Displays the admin dashboard with the number of pending accounts.
        /// </summary>
        /// <returns>The admin dashboard view.</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            // Count unread notifications related to account creation
            var pendingCount = await _notificationRepository
            .CountAsync(n => !n.IsRead && n.Message.Contains("Please create the user account"));

            // Pass the count to the view
            ViewData["PendingAccountsCount"] = pendingCount;
            return View();
        }

        /// <summary>
        /// Lists all users along with their assigned roles.
        /// </summary>
        /// <returns>A view displaying all users and their roles.</returns>
        public async Task<IActionResult> ManageUsers()
        {
            // Get all users
            var users = _userManager.Users.ToList();
            var model = new List<UserWithRolesViewModel>();

            // For each user, retrieve their assigned roles
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

        /// <summary>
        /// Displays the form for editing an existing user.
        /// </summary>
        /// <param name="id">The ID of the user to edit.</param>
        /// <returns>The user edit view.</returns>
        public async Task<IActionResult> EditUser(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            // Find user by ID
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            // Populate view model with user data
            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                EmailConfirmed = user.EmailConfirmed
            };

            return View(model);
        }

        /// <summary>
        /// Saves the changes made to a user.
        /// </summary>
        /// <param name="model">The updated user data.</param>
        /// <returns>Redirects to the user list or redisplays the form with validation errors.</returns>
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Find user by ID
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return NotFound();

            // Update user properties
            user.Email = model.Email;
            user.UserName = model.UserName;
            user.EmailConfirmed = model.EmailConfirmed;

            // Save changes
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return RedirectToAction(nameof(ManageUsers));

            // Handle update errors
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        /// <summary>
        /// Displays the confirmation view for deleting a user.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>The confirmation view for user deletion.</returns>
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            // Find user by ID
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        /// <summary>
        /// Deletes a user and the associated customer record, if one exists.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>Redirects to the user management view.</returns>
        [HttpPost, ActionName("DeleteUserConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserConfirmed(string id)
        {
            // Find user
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            // Check if there's a linked customer
            var customer = await _customerRepository.GetByUserIdAsync(user.Id);
            if (customer != null)
            {
                // Delete customer entity
                await _customerRepository.DeleteAsync(customer.Id);

            }

            // Delete user
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
                return RedirectToAction(nameof(ManageUsers));

            // Handle errors
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(user);
        }

        /// <summary>
        /// Displays the list of unread notifications for pending user accounts.
        /// </summary>
        /// <returns>A view with pending account notifications.</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PendingAccounts()
        {
            // Get unread notifications requesting account creation
            var notifications = await _notificationRepository
                .GetUnassignedAccountNotificationsAsync();

            return View(notifications);
        }

        /// <summary>
        /// Creates a user account for an approved meter request.
        /// Accessible only by employees.
        /// </summary>
        /// <param name="meterRequestId">The ID of the approved meter request.</param>
        /// <returns>Redirects to the pending requests view with a status message.</returns>
        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> CreateUserAccount(int meterRequestId)
        {
            // Find the approved meter request
            var request = await _context.MeterRequests.FindAsync(meterRequestId);
            if (request == null || request.Status != MeterRequestStatus.Approved)
            {
                return NotFound();
            }

            // Check if a user already exists with the same email
            var existingUser = await _userManager.FindByEmailAsync(request.RequesterEmail);
            if (existingUser != null)
            {
                TempData["StatusMessage"] = "A user with this email already exists.";
                return RedirectToAction("PendingRequests");
            }

            // Create a new ApplicationUser
            var newUser = new ApplicationUser
            {
                UserName = request.RequesterEmail,
                Email = request.RequesterEmail,
                EmailConfirmed = true // opcional
            };

            // Generate a temporary password
            var tempPassword = Guid.NewGuid().ToString("N").Substring(0, 8) + "!";
            var result = await _userManager.CreateAsync(newUser, tempPassword);

            if (!result.Succeeded)
            {
                TempData["StatusMessage"] = "Failed to create user account: " + string.Join(", ", result.Errors.Select(e => e.Description));
                return RedirectToAction("PendingRequests");
            }

            // Assign the "Customer" role
            await _userManager.AddToRoleAsync(newUser, "Customer");

            // Create and save Customer entity
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

            // Send password reset email
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(newUser);
            var resetLink = Url.Action("ResetPassword", "Account", new { token = resetToken, email = newUser.Email }, Request.Scheme);

            await _emailSender.SendEmailAsync(newUser.Email, "Your new account",
                $"Hello {newCustomer.FullName},<br><br>Your account has been created. Click <a href='{resetLink}'>here</a> to set your password.<br><br>Regards,<br>Water Billing App");

            TempData["StatusMessage"] = "User account created and email sent.";
            return RedirectToAction("PendingRequests");
        }

        /// <summary>
        /// Creates a user account based on a pending notification and links the user to the corresponding customer.
        /// Marks the notification as read after account creation.
        /// </summary>
        /// <param name="notificationId">The ID of the notification related to the customer.</param>
        /// <returns>Redirects to the pending accounts view with a status message.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUserFromNotification(int notificationId)
        {
            // Find the notification
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null || notification.IsRead)
                return NotFound();

            // Find the related customer
            var customer = await _customerRepository.GetByIdAsync(notification.CustomerId.Value);
            if (customer == null)
                return NotFound();

            // Check if a user already exists with the same email
            var existingUser = await _userManager.FindByEmailAsync(customer.Email);
            if (existingUser != null)
            {
                TempData["StatusMessage"] = "A user with this email already exists.";
                return RedirectToAction("PendingAccounts");
            }

            // Create a new ApplicationUser
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

            // Temporary password (for internal setup; will be replaced by user)
            var tempPassword = "Aa123456!";
            var result = await _userManager.CreateAsync(newUser, tempPassword);

            if (!result.Succeeded)
            {
                TempData["StatusMessage"] = "Failed to create account: " + string.Join(", ", result.Errors.Select(e => e.Description));
                return RedirectToAction("PendingAccounts");
            }

            // Assign "Customer" role
            await _userManager.AddToRoleAsync(newUser, "Customer");

            // Link the user to the customer
            customer.ApplicationUserId = newUser.Id;
            await _customerRepository.UpdateAsync(customer);

            // Send reset password link
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(newUser);
            var resetLink = Url.Action("ResetPassword", "Account", new { token = resetToken, email = newUser.Email }, Request.Scheme);

            await _emailSender.SendEmailAsync(newUser.Email, "Account Created",
                $"Hello {customer.FullName},<br><br>Your account has been created. Click <a href='{resetLink}'>here</a> to set your password.<br><br>Thank you.");

            // Mark notification as read
            notification.IsRead = true;
            await _notificationRepository.SaveChangesAsync();

            TempData["StatusMessage"] = "Account successfully created and notification marked as read.";
            return RedirectToAction("PendingAccounts");
        }

    }
}
