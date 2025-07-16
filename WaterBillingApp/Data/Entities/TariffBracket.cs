using System.ComponentModel.DataAnnotations;

namespace WaterBillingApp.Data.Entities
{
    /// <summary>
    /// Represents a tariff bracket defining pricing for a range of water consumption volumes.
    /// </summary>
    public class TariffBracket
    {
        /// <summary>
        /// Primary key of the tariff bracket.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Minimum volume (inclusive) for this tariff bracket.
        /// Required and must be between 0 and 99999.
        /// </summary>
        [Required]
        [Range(0, 99999)]
        public decimal MinVolume { get; set; }

        /// <summary>
        /// Maximum volume (inclusive) for this tariff bracket.
        /// Optional; null means no upper limit.
        /// Must be between 0 and 99999 if set.
        /// </summary>
        [Range(0, 99999)]
        public decimal? MaxVolume { get; set; }

        /// <summary>
        /// Price per cubic meter for consumption within this bracket.
        /// Required and must be between 0.01 and 100.
        /// </summary>
        [Required]
        [Range(0.01, 100)]
        public decimal PricePerCubicMeter { get; set; }
    }
}
