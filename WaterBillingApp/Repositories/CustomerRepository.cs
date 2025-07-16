using Microsoft.EntityFrameworkCore;
using WaterBillingApp.Data;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Helpers;

namespace WaterBillingApp.Repositories
{
    /// <summary>
    /// Repository for managing <see cref="Customer"/> entities, including
    /// customer-specific data access operations.
    /// Inherits generic repository functionality.
    /// </summary>
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves a customer entity by the associated user ID.
        /// </summary>
        /// <param name="userId">The application user ID.</param>
        /// <returns>The <see cref="Customer"/> associated with the specified user ID, or null if none found.</returns>
        public async Task<Customer?> GetByUserIdAsync(string userId)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);
        }

        /// <summary>
        /// Retrieves all customers including their related application user data.
        /// </summary>
        /// <returns>A list of all customers with their associated user information.</returns>
        public override async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _dbSet
                .Include(c => c.ApplicationUser)
                .ToListAsync();
        }
    }
}
