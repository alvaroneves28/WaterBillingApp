using WaterBillingApp.Data.Entities;

namespace WaterBillingApp.Helpers
{
    public interface IMeterRequestRepository
    {
        Task<List<MeterRequest>> GetPendingRequestsAsync();
        Task<MeterRequest?> GetByIdAsync(int id);
        Task UpdateAsync(MeterRequest meterRequest);
        Task AddAsync(MeterRequest meterRequest);
    }
}
