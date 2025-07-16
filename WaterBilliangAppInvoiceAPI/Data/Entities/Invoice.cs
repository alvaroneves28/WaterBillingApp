using WaterBillingApp.Data.Entities;

namespace WaterBilliangAppInvoiceAPI.Data.Entities
{
    public class Invoice
    {
        public int Id { get; set; }
        public decimal Total { get; set; }
        public DateTime IssuedAt { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
