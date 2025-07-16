using Microsoft.AspNetCore.Identity;

namespace WaterBillingApp.Data.Entities
{
    /// <summary>
    /// Represents an application user with extended profile information.
    /// Inherits from IdentityUser to include authentication and identity management.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// The customer entity associated with this user (if any).
        /// Used when the user also has a customer role.
        /// </summary>
        public Customer? Customer { get; set; }

        /// <summary>
        /// The file path or URL to the user's profile image.
        /// </summary>
        public string? ProfileImagePath { get; set; }

        /// <summary>
        /// The full name of the user (for display or administrative use).
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// The Portuguese tax identification number (NIF) of the user.
        /// </summary>
        public string NIF { get; set; }

        /// <summary>
        /// The address of the user (used for customer management or billing).
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// The user's contact phone number.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Indicates whether the user's account is active.
        /// Used to control access without deleting the user.
        /// </summary>
        public bool IsActive { get; set; }
    }
}
