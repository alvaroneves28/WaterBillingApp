namespace WaterBillingWebAPI.Model.DTO
{
    public class ConsumptionHistoryDTO
    {
        public int MeterId { get; set; }
        public DateTime Date { get; set; }
        public decimal Volume { get; set; }
    }
}
