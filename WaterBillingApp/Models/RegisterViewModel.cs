using System.ComponentModel.DataAnnotations;

namespace WaterBillingApp.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Nome Completo")]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "NIF")]
        public string NIF { get; set; }

        [Required]
        [Display(Name = "Morada")]
        [StringLength(200)]
        public string Address { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Telefone")]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "Ativo?")]
        public bool IsActive { get; set; } = true;

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "A password deve ter pelo menos 6 caracteres.")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "As passwords não coincidem.")]
        [Display(Name = "Confirmar Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }
    }
}
