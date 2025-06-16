using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Models;

namespace WaterBillingApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
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

            
            var clientes = _context.Customers.Where(c => c.ApplicationUserId == user.Id);
            _context.Customers.RemoveRange(clientes);

            
            await _context.SaveChangesAsync();

            
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
                return RedirectToAction(nameof(ManageUsers));

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(user);
        }



    }
}
