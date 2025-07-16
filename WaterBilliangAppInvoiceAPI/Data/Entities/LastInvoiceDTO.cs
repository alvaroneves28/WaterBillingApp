using WaterBillingApp.Data.Entities;

namespace WaterBilliangAppInvoiceAPI.Data.Entities
{
    public class LastInvoiceDTO
    {
        public int Id { get; set; }
        public DateTime IssueDate { get; set; }
        public InvoiceStatus Status { get; set; }
        public decimal ConsumptionValue { get; set; }
        public decimal TotalAmount { get; set; }
    }

}
