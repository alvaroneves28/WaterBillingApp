using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace WaterBillingApp.Data.Entities
{
    public class Customer
    {
        public int Id { get; set; }


        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        public string NIF { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public ICollection<Meter> Meters { get; set; }
    }
}
