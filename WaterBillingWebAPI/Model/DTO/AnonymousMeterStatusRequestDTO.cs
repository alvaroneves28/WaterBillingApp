using System.ComponentModel.DataAnnotations;

namespace WaterBillingWebAPI.Model.DTO
{
    public class AnonymousMeterStatusRequestDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string NIF { get; set; }
    }
}
