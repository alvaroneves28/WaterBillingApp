using System.ComponentModel.DataAnnotations;

namespace WaterBillingApp.Models
{
    /// <summary>
    /// ViewModel for meter installation requests.
    /// </summary>
    public class MeterRequestViewModel
    {
        /// <summary>
        /// Unique identifier for the meter request.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the requester.
        /// </summary>
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        /// <summary>
        /// Email address of the requester.
        /// </summary>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        /// <summary>
        /// Tax Identification Number (NIF) of the requester.
        /// </summary>
        [Required(ErrorMessage = "NIF is required.")]
        public string NIF { get; set; }

        /// <summary>
        /// Address related to the meter request.
        /// </summary>
        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        /// <summary>
        /// Contact phone number of the requester.
        /// </summary>
        [Required(ErrorMessage = "Phone is required.")]
        public string Phone { get; set; }

        /// <summary>
        /// Date when the request was made.
        /// </summary>
        public DateTime RequestDate { get; set; }

        /// <summary>
        /// Current status of the meter request.
        /// </summary>
        public string Status { get; set; }
    }
}
