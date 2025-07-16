using System.ComponentModel.DataAnnotations;

namespace WaterBillingApp.Models
{
    /// <summary>
    /// ViewModel representing a tariff bracket for water billing.
    /// </summary>
    public class TariffBracketViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier of the tariff bracket.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the minimum volume (inclusive) for this tariff bracket.
        /// Must be zero or greater.
        /// </summary>
        [Required]
        [Range(0, double.MaxValue)]
        public double MinVolume { get; set; }

        /// <summary>
        /// Gets or sets the maximum volume (inclusive) for this tariff bracket.
        /// Optional; can be null to indicate no upper limit.
        /// </summary>
        [Range(0, double.MaxValue)]
        public double? MaxVolume { get; set; }

        /// <summary>
        /// Gets or sets the price charged per cubic meter within this bracket.
        /// Must be between 0.01 and 100.
        /// </summary>
        [Required]
        [Range(0.01, 100)]
        public decimal PricePerCubicMeter { get; set; }
    }
}
