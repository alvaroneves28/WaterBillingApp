using Microsoft.EntityFrameworkCore;
using WaterBillingApp.Data;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Repositories;

namespace WaterBillingApp.Helpers
{
    /// <summary>
    /// Repository for managing <see cref="MeterRequest"/> entities.
    /// </summary>
    public class MeterRequestRepository : GenericRepository<MeterRequest>, IMeterRequestRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeterRequestRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public MeterRequestRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all meter requests that have a status of pending.
        /// </summary>
        /// <returns>A list of pending meter requests.</returns>
        public async Task<List<MeterRequest>> GetPendingRequestsAsync()
        {
            return await _context.MeterRequests
                .Where(mr => mr.Status == MeterRequestStatus.Pending)
                .ToListAsync();
        }
    }
}
