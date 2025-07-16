using System.ComponentModel.DataAnnotations;

/// <summary>
/// ViewModel for editing user details.
/// </summary>
public class EditUserViewModel
{
    /// <summary>
    /// User's unique identifier.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// User's email address.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    /// <summary>
    /// User's username.
    /// </summary>
    [Required]
    public string UserName { get; set; }

    /// <summary>
    /// Indicates whether the user's email has been confirmed.
    /// </summary>
    public bool EmailConfirmed { get; set; }

    // Additional properties

    /// <summary>
    /// User's full name.
    /// </summary>
    [Display(Name = "Full Name")]
    public string FullName { get; set; }

    /// <summary>
    /// User's tax identification number (NIF).
    /// </summary>
    [Display(Name = "NIF")]
    public string NIF { get; set; }

    /// <summary>
    /// User's postal address.
    /// </summary>
    [Display(Name = "Address")]
    public string Address { get; set; }

    /// <summary>
    /// User's contact phone number.
    /// </summary>
    [Display(Name = "Contact")]
    public string Phone { get; set; }

    /// <summary>
    /// Indicates if the user is active.
    /// </summary>
    [Display(Name = "Active")]
    public bool IsActive { get; set; }
}
