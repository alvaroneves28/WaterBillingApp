using WaterBillingApp.Data.Entities;

namespace WaterBillingApp.Models
{
    public class CustomerInvoicesViewModel
    {
        public string CustomerName { get; set; } = "";
        public List<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
