using System.ComponentModel.DataAnnotations;

namespace WaterBillingWebAPI.Model.DTO
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
