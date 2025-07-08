using WaterBillingApp.Data.Entities;

namespace WaterBillingApp.Models
{
    public class MetersDashboardViewModel
    {
        public IEnumerable<MeterViewModel> PendingMeters { get; set; }
        public IEnumerable<MeterViewModel> ActiveMeters { get; set; }

        public IEnumerable<MeterRequestViewModel> PendingMeterRequests { get; set; }
    }
}
