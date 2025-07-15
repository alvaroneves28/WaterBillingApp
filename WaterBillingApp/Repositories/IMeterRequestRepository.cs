using WaterBillingApp.Data.Entities;
using WaterBillingApp.Repositories;

namespace WaterBillingApp.Helpers
{
    public interface IMeterRequestRepository : IGenericRepository<MeterRequest>
    {
        Task<List<MeterRequest>> GetPendingRequestsAsync();
      
    }
}
