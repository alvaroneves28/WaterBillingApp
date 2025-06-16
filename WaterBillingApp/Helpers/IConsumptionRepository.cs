using WaterBillingApp.Data.Entities;

namespace WaterBillingApp.Helpers
{
    public interface IConsumptionRepository
    {
        Task<IEnumerable<Consumption>> GetAllAsync();
        Task<Consumption> GetByIdAsync(int id);
        Task<IEnumerable<Consumption>> GetByMeterIdAsync(int meterId);
        Task AddAsync(Consumption consumption);
        Task UpdateAsync(Consumption consumption);
        Task DeleteAsync(int id);
    }
}
