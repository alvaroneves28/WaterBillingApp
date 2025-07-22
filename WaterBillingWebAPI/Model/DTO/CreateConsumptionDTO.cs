namespace WaterBillingWebAPI.Model.DTO
{
    public class CreateConsumptionDTO
    {
        public int MeterId { get; set; }
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
    }
}
