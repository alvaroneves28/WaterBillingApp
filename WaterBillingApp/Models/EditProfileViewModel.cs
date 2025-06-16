using System.ComponentModel.DataAnnotations;

namespace WaterBillingApp.Models
{
    public class EditProfileViewModel
    {
        public string? Email { get; set; }

        [Display(Name = "Profile Photo")]
        public IFormFile? ProfileImage { get; set; }

        public string? ExistingImagePath { get; set; }
    }
}
