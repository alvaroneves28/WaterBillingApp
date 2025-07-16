using System.ComponentModel.DataAnnotations;

namespace WaterBillingApp.Models
{
    /// <summary>
    /// ViewModel used for the Forgot Password feature.
    /// </summary>
    public class ForgotPasswordViewModel
    {
        /// <summary>
        /// The email address of the user requesting a password reset.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
