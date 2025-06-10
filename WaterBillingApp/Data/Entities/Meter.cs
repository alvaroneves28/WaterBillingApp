using System.ComponentModel.DataAnnotations;

namespace REMOVED.Data.Entities
{
    public class Meter
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string SerialNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime InstallationDate { get; set; }

        [Required]
        public bool IsActive { get; set; }
        [Required]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public ICollection<Consumption> Consumptions { get; set; } = new List<Consumption>();
    }
}
