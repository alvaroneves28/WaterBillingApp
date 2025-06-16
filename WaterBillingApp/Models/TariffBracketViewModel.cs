using System.ComponentModel.DataAnnotations;

namespace WaterBillingApp.Models
{
    public class TariffBracketViewModel
    {
        public int Id { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public double MinVolume { get; set; }

        [Range(0, double.MaxValue)]
        public double? MaxVolume { get; set; }

        [Required]
        [Range(0.01, 100)]
        public decimal PricePerCubicMeter { get; set; }
    }
}
