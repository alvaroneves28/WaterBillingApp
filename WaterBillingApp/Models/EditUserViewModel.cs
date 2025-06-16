using System.ComponentModel.DataAnnotations;

public class EditUserViewModel
{
    public string Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string UserName { get; set; }

    public bool EmailConfirmed { get; set; }

    // Novas propriedades
    [Display(Name = "Full Name")]
    public string FullName { get; set; }

    [Display(Name = "NIF")]
    public string NIF { get; set; }

    [Display(Name = "Address")]
    public string Address { get; set; }

    [Display(Name = "Contact")]
    public string Phone { get; set; }

    [Display(Name = "Ativ")]
    public bool IsActive { get; set; }
}
