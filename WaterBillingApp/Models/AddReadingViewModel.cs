using System.ComponentModel.DataAnnotations;

namespace WaterBillingApp.Models
{
    /// <summary>
    /// ViewModel for adding a new meter reading,
    /// including meter info, consumption volume, and date.
    /// </summary>
    public class AddReadingViewModel
    {
        /// <summary>
        /// The ID of the meter this reading belongs to.
        /// Required for associating the reading with a specific meter.
        /// </summary>
        [Required]
        public int MeterId { get; set; }

        /// <summary>
        /// The serial number of the meter.
        /// Optional display-only property for UI purposes.
        /// </summary>
        [Display(Name = "Meter Serial Number")]
        public string? SerialNumber { get; set; }

        /// <summary>
        /// The volume of water consumed, in cubic meters.
        /// Required and must be between 0.01 and 100,000.
        /// </summary>
        [Required(ErrorMessage = "Volume is required.")]
        [Range(0.01, 100000, ErrorMessage = "Volume must be between 0.01 and 100000.")]
        [Display(Name = "Volume (m³)")]
        public decimal Volume { get; set; }

        /// <summary>
        /// The meter reading value.
        /// Optional in this model, but usually an integer.
        /// </summary>
        public int Reading { get; set; }

        /// <summary>
        /// The date the reading was taken.
        /// Required and displayed with a date picker in the UI.
        /// </summary>
        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Reading Date")]
        public DateTime Date { get; set; }
    }
}
