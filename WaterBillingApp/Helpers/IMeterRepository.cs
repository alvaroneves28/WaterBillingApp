using WaterBillingApp.Data.Entities;

namespace WaterBillingApp.Helpers
{
    public interface IMeterRepository
    {
        Task<IEnumerable<Meter>> GetAllAsync();
        Task<Meter?> GetByIdAsync(int id);
        Task AddAsync(Meter meter);
        Task UpdateAsync(Meter meter);
        Task DeleteAsync(int id);
        Task<IEnumerable<Meter>> GetPendingMetersAsync();
        Task<IEnumerable<Meter>> GetActiveMetersAsync();
        Task<IEnumerable<Meter>> GetMetersByCustomerAsync(int customerId);
        Task SaveAsync();

    }
}
