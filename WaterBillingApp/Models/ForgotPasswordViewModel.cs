﻿using System.ComponentModel.DataAnnotations;

namespace REMOVED.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
