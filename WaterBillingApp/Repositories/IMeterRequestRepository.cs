using WaterBillingApp.Data.Entities;
using WaterBillingApp.Repositories;

namespace WaterBillingApp.Helpers
{
    /// <summary>
    /// Interface for repository managing <see cref="MeterRequest"/> entities.
    /// </summary>
    public interface IMeterRequestRepository : IGenericRepository<MeterRequest>
    {
        /// <summary>
        /// Retrieves a list of meter requests with pending status.
        /// </summary>
        /// <returns>A list of pending <see cref="MeterRequest"/> objects.</returns>
        Task<List<MeterRequest>> GetPendingRequestsAsync();
    }
}
