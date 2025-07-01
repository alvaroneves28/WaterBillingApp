using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Models; // onde vais criar o viewmodel

[Authorize]
public class ManageController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly BlobServiceClient _blobServiceClient;

    public ManageController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, BlobServiceClient blobServiceClient)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _blobServiceClient = blobServiceClient;
    }

    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (result.Succeeded)
        {
            await _signInManager.RefreshSignInAsync(user);
            TempData["StatusMessage"] = "Your password has been changed.";
            return RedirectToAction("ChangePassword");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            TempData["ErrorMessage"] = "Could not change your password. Please try again.";
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult UploadProfilePicture()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UploadProfilePicture(IFormFile profilePicture)
    {
        if (profilePicture == null || profilePicture.Length == 0)
        {
            ModelState.AddModelError("", "Please select a valid image file.");
            return View();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound();

       
        var containerClient = _blobServiceClient.GetBlobContainerClient("appstorage28");
        await containerClient.CreateIfNotExistsAsync();

      
        var fileName = $"{Guid.NewGuid()}_{profilePicture.FileName}";
        var blobClient = containerClient.GetBlobClient($"images/{fileName}");

      
        using (var stream = profilePicture.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, overwrite: true);
        }

        user.ProfileImagePath = blobClient.Uri.ToString();
        await _userManager.UpdateAsync(user);

        TempData["StatusMessage"] = "Profile picture updated successfully.";
        return RedirectToAction("UploadProfilePicture", "Manage");
    }

}
