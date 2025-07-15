using Microsoft.EntityFrameworkCore;
using WaterBillingApp.Data;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Helpers;

namespace WaterBillingApp.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Customer?> GetByUserIdAsync(string userId)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);
        }

        
        public override async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _dbSet
                .Include(c => c.ApplicationUser)
                .ToListAsync();
        }
    }
}
