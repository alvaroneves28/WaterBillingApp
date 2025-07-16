using System.ComponentModel.DataAnnotations;

namespace WaterBillingApp.Models
{
    /// <summary>
    /// ViewModel used to add a new consumption record.
    /// Contains the meter ID, the reading value, and the date of the reading.
    /// </summary>
    public class AddConsumptionViewModel
    {
        /// <summary>
        /// The identifier of the meter to which this consumption belongs.
        /// </summary>
        public int MeterId { get; set; }

        /// <summary>
        /// The consumption reading value.
        /// Must be a non-negative integer.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid reading.")]
        public int Reading { get; set; }

        /// <summary>
        /// The date the consumption was recorded.
        /// Defaults to today's date.
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;
    }
}
