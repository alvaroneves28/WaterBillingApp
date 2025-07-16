using System.ComponentModel.DataAnnotations;

namespace WaterBillingApp.Models
{
    /// <summary>
    /// ViewModel representing customer details for display or editing.
    /// </summary>
    public class CustomerViewModel
    {
        /// <summary>
        /// Unique identifier of the customer.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Full name of the customer.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        /// <summary>
        /// Customer’s tax identification number (NIF).
        /// </summary>
        [Required]
        public string NIF { get; set; }

        /// <summary>
        /// Customer’s email address.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Customer’s physical address.
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        /// <summary>
        /// Indicates if the customer is active.
        /// </summary>
        [Required]
        public bool IsActive { get; set; }

        /// <summary>
        /// Customer’s phone number.
        /// </summary>
        [Required]
        [Phone]
        public string Phone { get; set; }
    }
}
