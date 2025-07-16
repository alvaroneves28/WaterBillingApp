using System.ComponentModel.DataAnnotations;

namespace WaterBillingApp.Models
{
    /// <summary>
    /// ViewModel for user registration with validation attributes.
    /// </summary>
    public class RegisterViewModel
    {
        /// <summary>
        /// Gets or sets the user email address.
        /// Required and must be a valid email format.
        /// </summary>
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the full name of the user.
        /// Required with a maximum length of 100 characters.
        /// </summary>
        [Required]
        [Display(Name = "Full Name")]
        [StringLength(100)]
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the Tax Identification Number (NIF).
        /// Required field.
        /// </summary>
        [Required]
        [Display(Name = "NIF")]
        public string NIF { get; set; }

        /// <summary>
        /// Gets or sets the user's address.
        /// Required with a maximum length of 200 characters.
        /// </summary>
        [Required]
        [Display(Name = "Address")]
        [StringLength(200)]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the user's phone number.
        /// Required and must be a valid phone number.
        /// </summary>
        [Required]
        [Phone]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is active.
        /// Required field, defaults to true.
        /// </summary>
        [Required]
        [Display(Name = "Active?")]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the password.
        /// Required, minimum length of 6 characters.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "The password must be at least 6 characters long.")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the password confirmation.
        /// Must match the Password field.
        /// </summary>
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Gets or sets the user role.
        /// Required field.
        /// </summary>
        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }
    }
}
