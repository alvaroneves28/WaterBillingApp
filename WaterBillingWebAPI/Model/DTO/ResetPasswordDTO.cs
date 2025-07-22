using System.ComponentModel.DataAnnotations;

namespace WaterBillingWebAPI.Model.DTO
{
    public class ResetPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "The password must be at least 6 characters long.")]
        public string NewPassword { get; set; }
    }
}
