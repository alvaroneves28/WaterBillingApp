using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WaterBillingApp.Data.Entities;
using WaterBillingWebAPI.Data;
using WaterBillingWebAPI.Data.Entities;

namespace WaterBillingWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _context.Users
                .Include(u => u.Customer)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound("User not found");

            return Ok(new
            {
                user.Id,
                user.FullName,
                user.Address,
                user.Email,
                Phone = user.Customer?.Phone ?? user.Phone, 
                user.ProfileImagePath,
                CustomerId = user.Customer?.Id
            });
        }


        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound("User not found");

            if (!string.IsNullOrEmpty(request.FullName))
                user.FullName = request.FullName;

            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, request.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    return BadRequest(setPhoneResult.Errors);
                }
            }

            if (!string.IsNullOrEmpty(request.Address))
            {
                user.Address = request.Address;  
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return BadRequest(updateResult.Errors);
            }

            return NoContent();
        }


        [HttpPut("email")]
        public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailRequest model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found");

            // Confirmar password atual
            var passwordValid = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
            if (!passwordValid)
                return BadRequest("Invalid current password.");

            // Atualizar email
            var setEmailResult = await _userManager.SetEmailAsync(user, model.NewEmail);
            if (!setEmailResult.Succeeded)
                return BadRequest(setEmailResult.Errors);

            // Atualizar nome de utilizador (UserName) para manter coerência (se for necessário)
            var setUserNameResult = await _userManager.SetUserNameAsync(user, model.NewEmail);
            if (!setUserNameResult.Succeeded)
                return BadRequest(setUserNameResult.Errors);

            return Ok("Email updated successfully.");
        }

        [HttpPut("password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found");

            var changePassResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!changePassResult.Succeeded)
                return BadRequest(changePassResult.Errors);

            return Ok("Password updated successfully.");
        }

        [HttpPut("image")]
        public async Task<IActionResult> UpdateProfileImage([FromBody] UpdateProfileImageRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound("User not found");

            user.ProfileImagePath = request.ProfileImageUrl;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }

    }
}
