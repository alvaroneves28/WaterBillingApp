using System.ComponentModel.DataAnnotations;

namespace WaterBillingApp.Data.Entities
{
    /// <summary>
    /// Represents a customer who has one or more water meters.
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// The unique identifier for the customer.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The full name of the customer.
        /// Maximum length is 100 characters.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        /// <summary>
        /// The customer's NIF (Tax Identification Number).
        /// </summary>
        [Required]
        public string NIF { get; set; }

        /// <summary>
        /// The customer's email address.
        /// Must be a valid email format.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// The customer's postal address.
        /// Maximum length is 200 characters.
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        /// <summary>
        /// Indicates whether the customer is currently active.
        /// </summary>
        [Required]
        public bool IsActive { get; set; }

        /// <summary>
        /// The customer's phone number.
        /// Must be a valid phone number format.
        /// </summary>
        [Required]
        [Phone]
        public string Phone { get; set; }

        /// <summary>
        /// The optional foreign key linking to the application user account.
        /// </summary>
        public string? ApplicationUserId { get; set; }

        /// <summary>
        /// Navigation property to the associated application user account.
        /// </summary>
        public ApplicationUser? ApplicationUser { get; set; }

        /// <summary>
        /// Collection of water meters assigned to this customer.
        /// </summary>
        public ICollection<Meter> Meters { get; set; }
    }
}
