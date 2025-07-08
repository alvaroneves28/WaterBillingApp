using WaterBillingApp.Data.Entities;

namespace WaterBillingApp.Models
{
    public class CustomerDashboardViewModel
    {
        public string CustomerName { get; set; } = string.Empty;

        public bool HasPendingInvoice { get; set; }

        public int? PendingInvoiceId { get; set; }


        public List<Notification> Notifications { get; set; } = new();
    }
}
