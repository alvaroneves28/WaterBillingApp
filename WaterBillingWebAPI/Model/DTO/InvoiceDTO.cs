using WaterBillingApp.Data.Entities;

namespace WaterBillingWebAPI.Model.DTO
{
    public class InvoiceDTO
    {
        public int Id { get; set; }
        public DateTime IssueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public InvoiceStatus Status { get; set; }
    }
}
