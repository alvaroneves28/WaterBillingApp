using System.ComponentModel.DataAnnotations;

namespace WaterBillingApp.Models
{
    /// <summary>
    /// ViewModel for user login data.
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// User's email address used for login.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// User's password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Indicates whether the user wants to be remembered on this device.
        /// </summary>
        public bool RememberMe { get; set; }
    }
}
