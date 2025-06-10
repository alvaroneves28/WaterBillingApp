using Microsoft.AspNetCore.Identity;

namespace REMOVED.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public Customer? Customer { get; set; }
    }
}
