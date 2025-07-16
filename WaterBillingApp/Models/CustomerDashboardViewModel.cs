using WaterBillingApp.Data.Entities;

namespace WaterBillingApp.Models
{
    /// <summary>
    /// ViewModel for displaying information on the customer's dashboard,
    /// including name, invoice status, and notifications.
    /// </summary>
    public class CustomerDashboardViewModel
    {
        /// <summary>
        /// Full name of the customer.
        /// </summary>
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if the customer has any pending invoices.
        /// </summary>
        public bool HasPendingInvoice { get; set; }

        /// <summary>
        /// The ID of the pending invoice, if any.
        /// </summary>
        public int? PendingInvoiceId { get; set; }

        /// <summary>
        /// List of notifications related to the customer.
        /// </summary>
        public List<Notification> Notifications { get; set; } = new();
    }
}
