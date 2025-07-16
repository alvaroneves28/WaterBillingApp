using WaterBillingApp.Data.Entities;

namespace WaterBillingApp.Models
{
    /// <summary>
    /// ViewModel representing a customer's invoices along with their name.
    /// </summary>
    public class CustomerInvoicesViewModel
    {
        /// <summary>
        /// The full name of the customer.
        /// </summary>
        public string CustomerName { get; set; } = "";

        /// <summary>
        /// List of invoices associated with the customer.
        /// </summary>
        public List<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
