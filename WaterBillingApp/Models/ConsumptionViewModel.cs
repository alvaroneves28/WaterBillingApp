namespace WaterBillingApp.Models
{
    /// <summary>
    /// ViewModel representing a consumption record,
    /// used to transfer consumption data to views.
    /// </summary>
    public class ConsumptionViewModel
    {
        /// <summary>
        /// Unique identifier for the consumption record.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Meter reading value at the time of consumption.
        /// </summary>
        public int Reading { get; set; }

        /// <summary>
        /// Volume of water consumed.
        /// </summary>
        public decimal Volume { get; set; }

        /// <summary>
        /// Date when the consumption was recorded.
        /// </summary>
        public DateTime Date { get; set; }
    }
}
