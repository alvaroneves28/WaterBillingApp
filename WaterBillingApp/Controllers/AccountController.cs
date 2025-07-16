using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Helpers;
using WaterBillingApp.Models;

namespace WaterBillingApp.Controllers
{
    /// <summary>
    /// Handles user account operations such as login, registration, password reset, and profile editing.
    /// </summary>
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            IEmailSender emailSender,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _emailSender = emailSender;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Displays the login form.
        /// </summary>
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Processes user login attempts.
        /// </summary>
        /// <param name="model">Login form data.</param>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // User not found with this email, so show generic error to avoid revealing info
                ModelState.AddModelError("", "Incorrect e-mail or password.");
                return View(model);
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                // Block login if email is not confirmed yet
                ModelState.AddModelError("", "E-mail not yet confirmed. Please check your Inbox.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                // Redirect users based on their assigned roles
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                    return RedirectToAction("Index", "Admin");
                else if (await _userManager.IsInRoleAsync(user, "Customer"))
                    return RedirectToAction("Index", "CustomerArea");
                else if (await _userManager.IsInRoleAsync(user, "Employee"))
                    return RedirectToAction("Index", "Employee");
                else
                    return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Failed Login.");
            return View(model);
        }

        /// <summary>
        /// Signs out the current user.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Displays the user registration form. Only accessible to admins.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Processes new user registration. Admins only.
        /// </summary>
        /// <param name="model">Registration data.</param>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    NIF = model.NIF,
                    Address = model.Address,
                    Phone = model.Phone,
                    IsActive = model.IsActive
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Assign selected role to the user
                    await _userManager.AddToRoleAsync(user, model.Role);

                    if (model.Role == "Customer")
                    {
                        // If customer role, create associated customer entity and link to user
                        var customer = new Customer
                        {
                            FullName = model.FullName,
                            NIF = model.NIF,
                            Email = model.Email,
                            Address = model.Address,
                            Phone = model.Phone,
                            IsActive = model.IsActive,
                            ApplicationUserId = user.Id
                        };
                        _context.Customers.Add(customer);
                        await _context.SaveChangesAsync();

                        user.Customer = customer;
                        await _userManager.UpdateAsync(user);
                    }

                    // Generate email confirmation token and send confirmation email
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var encodedToken = WebUtility.UrlEncode(token);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = encodedToken }, protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                        $"Please confirm your account by clicking here: <a href='{callbackUrl}'>Confirm email</a>");

                    ViewBag.StatusMessage = "User successfully registered. A confirmation email has been sent.";
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        /// <summary>
        /// Confirms the user's email using a token.
        /// </summary>
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            try
            {
                if (userId == null || token == null)
                    return RedirectToAction("Index", "Home");

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return NotFound($"Unable to load user with ID '{userId}'.");

                var decodedToken = WebUtility.UrlDecode(token);
                var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

                if (result.Succeeded)
                {
                    // Automatically redirect user to reset password after email confirmation
                    var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var encodedResetToken = WebUtility.UrlEncode(resetToken);
                    return RedirectToAction("ResetPassword", new { token = encodedResetToken, email = user.Email });
                }
                else
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new Exception("Error confirming email: " + errors);
                }
            }
            catch (Exception ex)
            {
                var model = new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    ErrorMessage = ex.Message
                };
                return View("Error", model);
            }
        }

        /// <summary>
        /// Displays the forgot password form.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Sends a reset password email if the user exists and email is confirmed.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                TempData["StatusMessage"] = "If an account with that email exists, a reset link has been sent.";
                return RedirectToAction(nameof(ForgotPassword));
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { token, email = model.Email }, protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                $"Please reset your password by <a href='{callbackUrl}'>clicking here</a>.");

            TempData["StatusMessage"] = "If an account with that email exists, a reset link has been sent.";
            return RedirectToAction(nameof(ForgotPassword));
        }

        /// <summary>
        /// Displays confirmation message after a forgot password request.
        /// </summary>
        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        /// <summary>
        /// Displays the reset password form with token and email.
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return View("Error", new ErrorViewModel { RequestId = "Invalid token or email." });
            }

            return View(new ResetPasswordViewModel { Token = token, Email = email });
        }

        /// <summary>
        /// Resets the user's password.
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                TempData["StatusMessage"] = "Password reset completed.";
                return RedirectToAction(nameof(Login));
            }

            var decodedToken = WebUtility.UrlDecode(model.Token);
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);

            if (result.Succeeded)
            {
                TempData["ResetPasswordStatus"] = "Password reset successful.";
                return RedirectToAction(nameof(Login));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        /// <summary>
        /// Displays password reset confirmation.
        /// </summary>
        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        /// <summary>
        /// Displays the Access Denied view when user lacks permissions.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// Updates the current user's profile, including profile picture.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (model.ProfileImage != null && model.ProfileImage.Length > 0)
            {
                // Save uploaded profile image to wwwroot/images/profiles with unique file name
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/profiles");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImage.CopyToAsync(fileStream);
                }

                // Update user profile image path to saved location
                user.ProfileImagePath = "/images/profiles/" + uniqueFileName;
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Error updating profile.");
                return View(model);
            }

            return RedirectToAction("Profile");
        }
    }
}
