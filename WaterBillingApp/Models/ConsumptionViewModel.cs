namespace WaterBillingApp.Models
{
    public class ConsumptionViewModel
    {
        public int Id { get; set; }

        public int Reading { get; set; }
        public decimal Volume { get; set; }
        public DateTime Date { get; set; }
    }
}
