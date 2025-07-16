using System.ComponentModel.DataAnnotations;

namespace WaterBillingApp.Data.Entities
{
    /// <summary>
    /// Enumeration representing the status of a meter.
    /// </summary>
    public enum MeterStatus
    {
        Pending,
        Approved,
        Rejected
    }

    /// <summary>
    /// Represents a water meter assigned to a customer.
    /// </summary>
    public class Meter
    {
        /// <summary>
        /// Primary key of the meter.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Unique serial number identifying the meter.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string SerialNumber { get; set; }

        /// <summary>
        /// Date when the meter was installed.
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        public DateTime InstallationDate { get; set; }

        /// <summary>
        /// Current status of the meter (Pending, Approved, or Rejected).
        /// Defaults to Pending.
        /// </summary>
        [Required]
        public MeterStatus Status { get; set; } = MeterStatus.Pending;

        /// <summary>
        /// Indicates whether the meter is active.
        /// </summary>
        [Required]
        public bool IsActive { get; set; }

        /// <summary>
        /// Foreign key to the customer who owns the meter.
        /// </summary>
        [Required]
        public int CustomerId { get; set; }

        /// <summary>
        /// Navigation property to the customer entity.
        /// </summary>
        public Customer Customer { get; set; }

        /// <summary>
        /// Collection of consumption records associated with this meter.
        /// </summary>
        public virtual ICollection<Consumption> Consumptions { get; set; } = new List<Consumption>();
    }
}
