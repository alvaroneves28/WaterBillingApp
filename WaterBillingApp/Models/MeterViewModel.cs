using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WaterBillingApp.Data.Entities;

namespace WaterBillingApp.Models
{
    /// <summary>
    /// ViewModel representing a meter and related information for display or editing.
    /// </summary>
    public class MeterViewModel
    {
        /// <summary>
        /// Unique identifier of the meter.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Serial number of the meter.
        /// </summary>
        [Required]
        [StringLength(50)]
        [Display(Name = "Serial Number")]
        public string SerialNumber { get; set; }

        /// <summary>
        /// Date when the meter was installed.
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Installation Date")]
        public DateTime InstallationDate { get; set; }

        /// <summary>
        /// Indicates whether the meter is active.
        /// </summary>
        [Required]
        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Identifier of the customer who owns the meter.
        /// </summary>
        [Required]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        /// <summary>
        /// Name of the customer owning the meter (optional, for display purposes).
        /// </summary>
        public string? CustomerName { get; set; }

        /// <summary>
        /// Current status of the meter (Pending, Approved, Rejected).
        /// </summary>
        public MeterStatus Status { get; set; }

        /// <summary>
        /// List of customers for selection in dropdowns (used in views).
        /// </summary>
        public IEnumerable<SelectListItem>? CustomersList { get; set; }

        /// <summary>
        /// The value of the last recorded consumption for this meter (if any).
        /// </summary>
        public decimal? LastConsumptionValue { get; set; }

        /// <summary>
        /// Date of the last recorded consumption for this meter (if any).
        /// </summary>
        public DateTime? LastConsumptionDate { get; set; }

        /// <summary>
        /// The full last consumption record details (optional).
        /// </summary>
        public ConsumptionViewModel LastConsumption { get; set; }
    }
}
