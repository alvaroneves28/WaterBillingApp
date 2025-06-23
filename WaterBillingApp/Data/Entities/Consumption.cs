using System.ComponentModel.DataAnnotations;

namespace WaterBillingApp.Data.Entities
{
    public class Consumption
    {
        public int Id { get; set; }

        [Required]
        [Range(0.01, 100000)]
        public decimal Volume { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        public int MeterId { get; set; }
        public Meter Meter { get; set; }

        public Invoice? Invoice { get; set; }

        public int Reading { get; set; }

        public int TariffBracketId { get; set; }
        public TariffBracket TariffBracket { get; set; }
    }
}
