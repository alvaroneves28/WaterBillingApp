namespace WaterBilliangAppInvoiceAPI.Data.Entities
{
    public class Consumption
    {
        public int Id { get; set; }
        public decimal Volume { get; set; }  
        public int MeterId { get; set; }
        public Meter Meter { get; set; }
    }
}
