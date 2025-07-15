using Microsoft.EntityFrameworkCore;
using WaterBillingApp.Data;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Repositories;

namespace WaterBillingApp.Helpers
{
    public class MeterRequestRepository : GenericRepository<MeterRequest>, IMeterRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public MeterRequestRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<MeterRequest>> GetPendingRequestsAsync()
        {
            return await _context.MeterRequests
                .Where(mr => mr.Status == MeterRequestStatus.Pending)
                .ToListAsync();
        }
    }
}
