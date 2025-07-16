using WaterBillingApp.Data.Entities;

namespace WaterBillingApp.Models
{
    /// <summary>
    /// ViewModel that combines a user entity with their assigned roles.
    /// </summary>
    public class UserWithRolesViewModel
    {
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public ApplicationUser User { get; set; }

        /// <summary>
        /// Gets or sets the list of role names assigned to the user.
        /// </summary>
        public IList<string> Roles { get; set; }
    }
}
