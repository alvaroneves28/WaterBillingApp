using WaterBillingApp.Data.Entities;
using WaterBillingApp.Repositories;

namespace WaterBillingApp.Helpers
{
    /// <summary>
    /// Defines methods for customer-specific data operations,
    /// extending the generic repository interface.
    /// </summary>
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        /// <summary>
        /// Retrieves a customer by their associated application user ID.
        /// </summary>
        /// <param name="userId">The application user ID.</param>
        /// <returns>The customer linked to the specified user ID, or null if not found.</returns>
        Task<Customer?> GetByUserIdAsync(string userId);
    }
}
