namespace WaterBillingWebAPI.Model.DTO
{
    public class TariffDTO
    {
        public decimal MinVolume { get; set; }
        public decimal? MaxVolume { get; set; }
        public decimal PricePerCubicMeter { get; set; }
    }
}
