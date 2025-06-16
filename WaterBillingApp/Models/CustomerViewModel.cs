using System.ComponentModel.DataAnnotations;

namespace WaterBillingApp.Models
{
    public class CustomerViewModel
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
    }
}
