using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Models;

/// <summary>
/// Controller for managing user account actions such as changing password and uploading profile pictures.
/// All actions require the user to be authenticated.
/// </summary>
[Authorize]
public class ManageController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly BlobServiceClient _blobServiceClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="ManageController"/> class.
    /// </summary>
    /// <param name="userManager">The user manager for handling user-related operations.</param>
    /// <param name="signInManager">The sign-in manager for managing authentication.</param>
    /// <param name="blobServiceClient">The Azure Blob Storage client for uploading files.</param>
    public ManageController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, BlobServiceClient blobServiceClient)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _blobServiceClient = blobServiceClient;
    }

    /// <summary>
    /// Displays the change password form.
    /// </summary>
    /// <returns>The ChangePassword view.</returns>
    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View(); // Returns the view that displays the password change form.
    }

    /// <summary>
    /// Handles the change password form submission.
    /// </summary>
    /// <param name="model">ViewModel containing current and new passwords.</param>
    /// <returns>Redirects to ChangePassword on success, or redisplays the form with errors on failure.</returns>
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model); // If the model is invalid, redisplay the form with validation errors.

        var user = await _userManager.GetUserAsync(User); // Gets the currently authenticated user.
        if (user == null)
        {
            return RedirectToAction("Login", "Account"); // If the user is not found, redirect to login page.
        }

        // Attempts to change the user's password.
        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (result.Succeeded)
        {
            await _signInManager.RefreshSignInAsync(user); // Re-signs in the user with the new credentials.
            TempData["StatusMessage"] = "Your password has been changed.";
            return RedirectToAction("ChangePassword"); // Redirect to the same page with success message.
        }
        else
        {
            // If the password change fails, add errors to the ModelState.
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            TempData["ErrorMessage"] = "Could not change your password. Please try again.";
            return View(model); // Redisplay the form with the errors.
        }
    }

    /// <summary>
    /// Displays the profile picture upload form.
    /// </summary>
    /// <returns>The UploadProfilePicture view.</returns>
    [HttpGet]
    public IActionResult UploadProfilePicture()
    {
        return View(); // Returns the view for uploading a profile picture.
    }

    /// <summary>
    /// Handles uploading and saving the profile picture to Azure Blob Storage.
    /// Updates the user's profile image URL in the database.
    /// </summary>
    /// <param name="profilePicture">The uploaded image file.</param>
    /// <returns>Redirects to the UploadProfilePicture view with status messages.</returns>
    [HttpPost]
    public async Task<IActionResult> UploadProfilePicture(IFormFile profilePicture)
    {
        // Validate the uploaded file.
        if (profilePicture == null || profilePicture.Length == 0)
        {
            ModelState.AddModelError("", "Please select a valid image file.");
            return View(); // Redisplay the form if file is invalid.
        }

        var user = await _userManager.GetUserAsync(User); // Get the currently authenticated user.
        if (user == null)
            return NotFound(); // If user is not found, return 404.

        // Get a reference to the Azure Blob Storage container.
        var containerClient = _blobServiceClient.GetBlobContainerClient("appstorage28");
        await containerClient.CreateIfNotExistsAsync(); // Create the container if it doesn't exist.

        // Generate a unique file name using a GUID.
        var fileName = $"{Guid.NewGuid()}_{profilePicture.FileName}";

        // Get a blob client for the new image inside the "images/" folder.
        var blobClient = containerClient.GetBlobClient($"images/{fileName}");

        // Upload the file to Azure Blob Storage.
        using (var stream = profilePicture.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, overwrite: true);
        }

        // Update the user's profile image path with the blob's URL.
        user.ProfileImagePath = blobClient.Uri.ToString();
        await _userManager.UpdateAsync(user); // Save changes to the user in the database.

        TempData["StatusMessage"] = "Profile picture updated successfully.";
        return RedirectToAction("UploadProfilePicture", "Manage"); // Redirect with success message.
    }

}
