using System.ComponentModel.DataAnnotations;

namespace WaterBillingApp.Models
{
    /// <summary>
    /// ViewModel used for changing a user's password,
    /// including validation for current, new, and confirmation passwords.
    /// </summary>
    public class ChangePasswordViewModel
    {
        /// <summary>
        /// The user's current password.
        /// Required for security verification.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }

        /// <summary>
        /// The new password the user wants to set.
        /// Must be at least 6 characters long.
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        /// <summary>
        /// Confirmation of the new password.
        /// Must match the new password field.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
