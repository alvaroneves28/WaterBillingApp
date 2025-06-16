using System.ComponentModel.DataAnnotations;

namespace WaterBillingApp.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
