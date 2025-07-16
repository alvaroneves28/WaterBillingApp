using WaterBillingApp.Data.Entities;

namespace WaterBillingApp.Models
{
    /// <summary>
    /// ViewModel for displaying meters and meter requests on the dashboard.
    /// </summary>
    public class MetersDashboardViewModel
    {
        /// <summary>
        /// Collection of meters that are pending approval.
        /// </summary>
        public IEnumerable<MeterViewModel> PendingMeters { get; set; }

        /// <summary>
        /// Collection of meters that are currently active.
        /// </summary>
        public IEnumerable<MeterViewModel> ActiveMeters { get; set; }

        /// <summary>
        /// Collection of pending meter installation requests.
        /// </summary>
        public IEnumerable<MeterRequestViewModel> PendingMeterRequests { get; set; }
    }
}
