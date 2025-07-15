using WaterBillingApp.Data.Entities;
using WaterBillingApp.Repositories;

namespace WaterBillingApp.Helpers
{
    public interface IMeterRepository : IGenericRepository<Meter>
    {
        Task<IEnumerable<Meter>> GetPendingMetersAsync();
        Task<IEnumerable<Meter>> GetActiveMetersAsync();
        Task<IEnumerable<Meter>> GetMetersByCustomerAsync(int customerId);
        Task SaveAsync();

    }
}
