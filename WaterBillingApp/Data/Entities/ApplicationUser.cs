using Microsoft.AspNetCore.Identity;

namespace WaterBillingApp.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public Customer? Customer { get; set; }
        public string? ProfileImagePath { get; set; }
        public string FullName { get; set; }
        public string NIF { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        

    }
}
