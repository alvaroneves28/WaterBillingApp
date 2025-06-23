using System.ComponentModel.DataAnnotations;

namespace WaterBillingApp.Models
{
    public class AddReadingViewModel
    {
        [Required]
        public int MeterId { get; set; }

        [Display(Name = "Meter Serial Number")]
        public string? SerialNumber { get; set; }

        [Required(ErrorMessage = "Volume is required.")]
        [Range(0.01, 100000, ErrorMessage = "Volume must be between 0.01 and 100000.")]
        [Display(Name = "Volume (m³)")]
        public decimal Volume { get; set; }

        public int Reading { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Reading Date")]
        public DateTime Date { get; set; }
    }
}
