using System.ComponentModel.DataAnnotations;

namespace WaterBillingApp.Models
{
    public class AddConsumptionViewModel
    {
        public int MeterId { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid reading.")]
        public int Reading { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;
    }
}
