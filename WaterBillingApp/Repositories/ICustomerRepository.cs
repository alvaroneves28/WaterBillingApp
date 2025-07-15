using WaterBillingApp.Data.Entities;
using WaterBillingApp.Repositories;

namespace WaterBillingApp.Helpers
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
       
        Task<Customer?> GetByUserIdAsync(string userId);
     
    }
}
