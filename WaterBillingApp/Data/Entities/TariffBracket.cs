using System.ComponentModel.DataAnnotations;

namespace WaterBillingApp.Data.Entities
{
    public class TariffBracket
    {
        public int Id { get; set; }

        [Required]
        [Range(0, 99999)]
        public decimal MinVolume { get; set; }

        [Range(0, 99999)]
        public decimal? MaxVolume { get; set; }

        [Required]
        [Range(0.01, 100)]
        public decimal PricePerCubicMeter { get; set; }
    }
}
