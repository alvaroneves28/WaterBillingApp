using System.ComponentModel.DataAnnotations;

namespace WaterBillingApp.Models
{
    public class MeterRequestViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "NIF is required.")]
        public string NIF { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Phone is required.")]
        public string Phone { get; set; }

        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
    }
}
