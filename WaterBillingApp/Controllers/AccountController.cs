using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REMOVED.Data;
using REMOVED.Data.Entities;
using REMOVED.Helpers;
using REMOVED.Models;

namespace REMOVED.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // 🔍 Obter o utilizador pelo email
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Incorrect e-mail or password.");
                return View(model);
            }

            // ❌ Verificar se o email foi confirmado
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("", "E-mail not yet confirmed. Please check your Inbox.");
                return View(model);
            }

            // ✔️ Autenticar
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                // Redirecionar conforme o papel do utilizador
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    return RedirectToAction("Index", "Admin"); // Painel admin
                }

                return RedirectToAction("Index", "Home"); // Utilizador normal
            }

            ModelState.AddModelError("", "Failed Login.");
            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
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
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Atribuir role ao user
                    await _userManager.AddToRoleAsync(user, model.Role);

                    // Criar o Customer (se for o caso)
                    if (model.Role == "Customer")
                    {
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

                    //  Gerar token de confirmação e enviar por email
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new
                    {
                        userId = user.Id,
                        token = token
                    }, protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(user.Email, "Confirmação de Email",
                        $"<p>Bem-vindo ao REMOVED!</p><p>Por favor confirme o seu email clicando neste <a href='{confirmationLink}'>link</a>.</p>");

                    ViewBag.StatusMessage = "Utilizador registado com sucesso. Um email de confirmação foi enviado.";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not Found.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                TempData["StatusMessage"] = "Email successfully confirmed. You may now log in.";
                return RedirectToAction("Login");
            }

            TempData["StatusMessage"] = "E-mail confirmation failed.";
            return RedirectToAction("Login");
        }


        // GET: /Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Para não revelar se o email existe ou não
                TempData["StatusMessage"] = "If an account with that email exists, a reset link has been sent.";
                return RedirectToAction(nameof(ForgotPassword));
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new
            {
                token,
                email = model.Email
            }, protocol: Request.Scheme);

            var emailMessage = $"Please reset your password by <a href='{callbackUrl}'>clicking here</a>.";

            await _emailSender.SendEmailAsync(
                model.Email,
                "Reset Password",
                emailMessage
            );

            TempData["StatusMessage"] = "If an account with that email exists, a reset link has been sent.";
            return RedirectToAction(nameof(ForgotPassword));
        }

        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: /Account/ResetPassword
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Não revelar se o utilizador existe
                TempData["StatusMessage"] = "Password reset completed.";
                return RedirectToAction(nameof(Login));
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                TempData["StatusMessage"] = "Password reset successful.";
                return RedirectToAction(nameof(Login));
            }

            // Mostrar erro genérico
            TempData["StatusMessage"] = "Could not reset password. Please try again.";
            return View(model);
        }


        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

    }
}
