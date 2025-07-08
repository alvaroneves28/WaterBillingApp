using WaterBillingApp.Data.Entities;

namespace WaterBillingApp.Helpers
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer> GetByIdAsync(int id);
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(int id);
        Task<Customer?> GetByUserIdAsync(string userId);
     
    }
}
