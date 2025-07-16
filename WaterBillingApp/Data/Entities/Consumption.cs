using System.ComponentModel.DataAnnotations;

namespace WaterBillingApp.Data.Entities
{
    /// <summary>
    /// Represents a record of water consumption associated with a specific meter.
    /// </summary>
    public class Consumption
    {
        /// <summary>
        /// The unique identifier for the consumption record.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The volume of water consumed (in cubic meters).
        /// Must be a positive decimal value.
        /// </summary>
        [Required]
        [Range(0.01, 100000)]
        public decimal Volume { get; set; }

        /// <summary>
        /// The date when the consumption reading was recorded.
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        /// <summary>
        /// The foreign key referencing the associated meter.
        /// </summary>
        [Required]
        public int MeterId { get; set; }

        /// <summary>
        /// Navigation property to the meter associated with this consumption.
        /// </summary>
        public Meter Meter { get; set; }

        /// <summary>
        /// The invoice that includes this consumption, if already billed.
        /// </summary>
        public Invoice? Invoice { get; set; }

        /// <summary>
        /// The actual meter reading value at the time of consumption.
        /// Used to calculate the difference since the last reading.
        /// </summary>
        public int Reading { get; set; }

        /// <summary>
        /// The ID of the tariff bracket applied to this consumption, if applicable.
        /// </summary>
        public int? TariffBracketId { get; set; }

        /// <summary>
        /// Navigation property to the tariff bracket used to calculate this consumption.
        /// </summary>
        public TariffBracket? TariffBracket { get; set; }
    }
}
