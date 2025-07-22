using System.ComponentModel.DataAnnotations;

namespace WaterBillingApp.Data.Entities
{
    /// <summary>
    /// Represents the possible statuses of an invoice.
    /// </summary>
    public enum InvoiceStatus
    {
        /// <summary>
        /// Invoice is pending and awaiting approval.
        /// </summary>
        Pending,

        /// <summary>
        /// Invoice has been approved.
        /// </summary>
        Approved,

        /// <summary>
        /// Invoice has been rejected.
        /// </summary>
        Rejected
    }

    /// <summary>
    /// Represents an invoice issued for a customer's water consumption.
    /// </summary>
    public class Invoice
    {
        /// <summary>
        /// The unique identifier of the invoice.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The date the invoice was issued.
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; }

        /// <summary>
        /// The total amount billed in the invoice.
        /// Must be between 0.01 and 100000.
        /// </summary>
        [Required]
        [Range(0.01, 100000)]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// The current status of the invoice.
        /// </summary>
        [Required]
        [StringLength(20)]
        public InvoiceStatus Status { get; set; }

        public bool IsRead { get; set; } = false;

        /// <summary>
        /// Foreign key to the associated consumption record.
        /// </summary>
        [Required]
        public int ConsumptionId { get; set; }

        /// <summary>
        /// Navigation property to the associated consumption.
        /// </summary>
        public Consumption Consumption { get; set; }
    }
}
