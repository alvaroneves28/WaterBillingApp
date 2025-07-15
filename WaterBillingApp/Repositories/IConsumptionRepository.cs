using WaterBillingApp.Data.Entities;
using WaterBillingApp.Repositories;

namespace WaterBillingApp.Helpers
{
    public interface IConsumptionRepository : IGenericRepository<Consumption>
    {
        
        Task<IEnumerable<Consumption>> GetByMeterIdAsync(int meterId);
        
        Task<bool> AnyByMeterIdAsync(int meterId);
    }
}
