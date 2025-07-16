using System.ComponentModel.DataAnnotations;

namespace WaterBillingApp.Models
{
    /// <summary>
    /// ViewModel for editing user profile information.
    /// </summary>
    public class EditProfileViewModel
    {
        /// <summary>
        /// User's email address.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// New profile photo uploaded by the user.
        /// </summary>
        [Display(Name = "Profile Photo")]
        public IFormFile? ProfileImage { get; set; }

        /// <summary>
        /// Path to the existing profile image (if any).
        /// </summary>
        public string? ExistingImagePath { get; set; }
    }
}
