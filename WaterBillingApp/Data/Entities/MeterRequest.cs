using System.ComponentModel.DataAnnotations;

namespace WaterBillingApp.Data.Entities
{
    public class MeterRequest
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime RequestDate { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; }
    }
}
