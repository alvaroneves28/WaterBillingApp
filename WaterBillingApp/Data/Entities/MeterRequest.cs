using System.ComponentModel.DataAnnotations;

namespace WaterBillingApp.Data.Entities
{
    /// <summary>
    /// Enumeration representing the status of a meter installation request.
    /// </summary>
    public enum MeterRequestStatus
    {
        Pending,
        Approved,
        Rejected
    }

    /// <summary>
    /// Represents a request made by a customer for a new meter installation.
    /// </summary>
    public class MeterRequest
    {
        /// <summary>
        /// Primary key of the meter request.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the person requesting the meter.
        /// </summary>
        [Required]
        public string RequesterName { get; set; }

        /// <summary>
        /// Email address of the requester.
        /// </summary>
        [Required]
        [EmailAddress]
        public string RequesterEmail { get; set; }

        /// <summary>
        /// Tax Identification Number (NIF) of the requester.
        /// </summary>
        [Required]
        public string NIF { get; set; }

        /// <summary>
        /// Address where the meter installation is requested.
        /// </summary>
        [Required]
        public string Address { get; set; }

        /// <summary>
        /// Phone number of the requester.
        /// </summary>
        [Required]
        [Phone]
        public string Phone { get; set; }

        /// <summary>
        /// Date when the request was submitted.
        /// </summary>
        public DateTime RequestDate { get; set; }

        /// <summary>
        /// Current status of the meter request.
        /// </summary>
        public MeterRequestStatus Status { get; set; }
    }
}
